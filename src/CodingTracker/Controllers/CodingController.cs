using CodingTracker.Data;
using CodingTracker.Models;
using CodingTracker.Services;

namespace CodingTracker.Controllers;

public sealed class CodingController(ICodingSessionRepository repository)
{
    public CodingSession Add(DateTime start, DateTime end)
    {
        EnsureRange(start, end);
        var session = new CodingSession { StartTime = start, EndTime = end };
        session.Id = repository.Create(session);
        return session;
    }

    public List<CodingSession> GetSessions(SessionFilter filter)
    {
        IEnumerable<CodingSession> sessions = repository.GetAll();
        if (filter.Period != Period.All)
        {
            var reference = (filter.ReferenceDate ?? DateTime.Today).Date;
            var (from, until) = GetBounds(filter.Period, reference);
            sessions = sessions.Where(s => s.StartTime >= from && s.StartTime < until);
        }

        sessions = filter.SortDirection == SortDirection.Ascending
            ? sessions.OrderBy(s => s.StartTime)
            : sessions.OrderByDescending(s => s.StartTime);
        return sessions.ToList();
    }

    public bool Update(int id, DateTime start, DateTime end)
    {
        EnsureRange(start, end);
        return repository.Update(new CodingSession { Id = id, StartTime = start, EndTime = end });
    }

    public bool Delete(int id) => repository.Delete(id);

    public static (DateTime From, DateTime Until) GetBounds(Period period, DateTime date) => period switch
    {
        Period.Day => (date.Date, date.Date.AddDays(1)),
        Period.Week => WeekBounds(date),
        Period.Month => (new DateTime(date.Year, date.Month, 1), new DateTime(date.Year, date.Month, 1).AddMonths(1)),
        Period.Year => (new DateTime(date.Year, 1, 1), new DateTime(date.Year + 1, 1, 1)),
        _ => (DateTime.MinValue, DateTime.MaxValue)
    };

    private static (DateTime, DateTime) WeekBounds(DateTime date)
    {
        var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
        var monday = date.Date.AddDays(-daysSinceMonday);
        return (monday, monday.AddDays(7));
    }

    private static void EnsureRange(DateTime start, DateTime end)
    {
        if (!Validation.IsValidRange(start, end))
            throw new ArgumentException("End time must be later than start time.");
    }
}
