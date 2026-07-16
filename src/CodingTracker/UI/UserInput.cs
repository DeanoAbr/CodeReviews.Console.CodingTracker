using CodingTracker.Controllers;
using CodingTracker.Models;
using CodingTracker.Services;
using Spectre.Console;

namespace CodingTracker.UI;

public sealed class UserInput(CodingController controller, IClock clock)
{
    public void Run()
    {
        AnsiConsole.Write(new FigletText("Coding Tracker").Color(Color.Aqua));
        var running = true;
        while (running)
        {
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[yellow]What would you like to do?[/]")
                .AddChoices("View sessions", "Add session manually", "Start stopwatch",
                    "Update session", "Delete session", "Exit"));
            try
            {
                switch (choice)
                {
                    case "View sessions": ViewSessions(); break;
                    case "Add session manually": AddSession(); break;
                    case "Start stopwatch": TrackWithStopwatch(); break;
                    case "Update session": UpdateSession(); break;
                    case "Delete session": DeleteSession(); break;
                    case "Exit": running = false; break;
                }
            }
            catch (Exception exception)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(exception.Message)}[/]");
            }
        }
    }

    private void ViewSessions()
    {
        var period = AnsiConsole.Prompt(new SelectionPrompt<Period>()
            .Title("Filter by period:").AddChoices(Enum.GetValues<Period>()));
        DateTime? reference = period == Period.All ? null : PromptDate("Reference date");
        var direction = AnsiConsole.Prompt(new SelectionPrompt<SortDirection>()
            .Title("Sort by start time:").AddChoices(Enum.GetValues<SortDirection>()));
        SessionTable.Show(controller.GetSessions(new SessionFilter(period, reference, direction)));
    }

    private void AddSession()
    {
        var (start, end) = PromptRange();
        var session = controller.Add(start, end);
        Success($"Session {session.Id} added ({FormatDuration(session.Duration)}).");
    }

    private void UpdateSession()
    {
        SessionTable.Show(controller.GetSessions(new SessionFilter()));
        var id = PromptId("Session ID to update");
        var (start, end) = PromptRange();
        ReportResult(controller.Update(id, start, end), "Session updated.", "Session not found.");
    }

    private void DeleteSession()
    {
        SessionTable.Show(controller.GetSessions(new SessionFilter()));
        var id = PromptId("Session ID to delete");
        if (!AnsiConsole.Confirm($"Delete session {id}?", false)) return;
        ReportResult(controller.Delete(id), "Session deleted.", "Session not found.");
    }

    private void TrackWithStopwatch()
    {
        AnsiConsole.MarkupLine("[yellow]Press Enter to start the timer.[/]");
        Console.ReadLine();
        var start = clock.Now;
        AnsiConsole.MarkupLine($"[green]Started at {start:dd-MM-yyyy HH:mm:ss}. Press Enter to stop.[/]");
        Console.ReadLine();
        var session = controller.Add(start, clock.Now);
        Success($"Session {session.Id} saved ({FormatDuration(session.Duration)}).");
    }

    private static (DateTime Start, DateTime End) PromptRange()
    {
        while (true)
        {
            var start = PromptDateTime("Start time");
            var end = PromptDateTime("End time");
            if (Validation.IsValidRange(start, end)) return (start, end);
            AnsiConsole.MarkupLine("[red]End time must be later than start time. Please try again.[/]");
        }
    }

    private static DateTime PromptDateTime(string label)
    {
        while (true)
        {
            var value = AnsiConsole.Ask<string>($"{label} [grey]({Validation.DateTimeFormat})[/]:");
            if (Validation.TryParseDateTime(value, out var result)) return result;
            AnsiConsole.MarkupLine($"[red]Use exactly {Validation.DateTimeFormat}, for example 16-07-2026 14:30.[/]");
        }
    }

    private static DateTime PromptDate(string label)
    {
        while (true)
        {
            var value = AnsiConsole.Ask<string>($"{label} [grey](dd-MM-yyyy)[/]:");
            if (DateTime.TryParseExact(value, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var result)) return result;
            AnsiConsole.MarkupLine("[red]Use exactly dd-MM-yyyy, for example 16-07-2026.[/]");
        }
    }

    private static int PromptId(string label)
    {
        while (true)
        {
            var value = AnsiConsole.Ask<string>($"{label}:");
            if (Validation.TryParsePositiveId(value, out var id)) return id;
            AnsiConsole.MarkupLine("[red]Enter a positive whole number.[/]");
        }
    }

    private static string FormatDuration(TimeSpan duration) =>
        $"{(int)duration.TotalHours:00}:{duration.Minutes:00}:{duration.Seconds:00}";
    private static void Success(string message) =>
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(message)}[/]");
    private static void ReportResult(bool result, string success, string failure) =>
        AnsiConsole.MarkupLine(result ? $"[green]{success}[/]" : $"[red]{failure}[/]");
}
