using CodingTracker.Controllers;
using CodingTracker.Data;
using CodingTracker.Models;

namespace CodingTracker.Tests;

public sealed class CodingControllerTests
{
    [Fact]
    public void GetSessions_FiltersCalendarWeekAndSortsAscending()
    {
        var repository = new FakeRepository(
            Session(1, new DateTime(2026, 7, 12)),
            Session(2, new DateTime(2026, 7, 19)),
            Session(3, new DateTime(2026, 7, 13)));
        var result = new CodingController(repository).GetSessions(new SessionFilter(
            Period.Week, new DateTime(2026, 7, 16), SortDirection.Ascending));
        Assert.Equal([3, 2], result.Select(x => x.Id));
    }

    [Fact]
    public void Add_CalculatesDurationFromTimes()
    {
        var controller = new CodingController(new FakeRepository());
        var start = new DateTime(2026, 7, 16, 9, 0, 0);
        Assert.Equal(TimeSpan.FromMinutes(90), controller.Add(start, start.AddMinutes(90)).Duration);
    }

    private static CodingSession Session(int id, DateTime start) =>
        new() { Id = id, StartTime = start, EndTime = start.AddHours(1) };

    private sealed class FakeRepository(params CodingSession[] seed) : ICodingSessionRepository
    {
        private readonly List<CodingSession> _sessions = [.. seed];
        public void InitializeDatabase() { }
        public int Create(CodingSession session) { session.Id = _sessions.Count + 1; _sessions.Add(session); return session.Id; }
        public List<CodingSession> GetAll() => [.. _sessions];
        public bool Update(CodingSession session) => true;
        public bool Delete(int id) => true;
    }
}
