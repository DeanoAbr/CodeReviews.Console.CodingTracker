using CodingTracker.Models;
using CodingTracker.Services;
using Spectre.Console;

namespace CodingTracker.UI;

public static class SessionTable
{
    public static void Show(IReadOnlyCollection<CodingSession> sessions)
    {
        if (sessions.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No coding sessions found.[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn("[aqua]ID[/]").AddColumn("[aqua]Start[/]")
            .AddColumn("[aqua]End[/]").AddColumn("[aqua]Duration[/]");
        foreach (var session in sessions)
            table.AddRow(session.Id.ToString(), session.StartTime.ToString(Validation.DateTimeFormat),
                session.EndTime.ToString(Validation.DateTimeFormat), FormatDuration(session.Duration));
        table.Caption = new TableTitle($"[grey]{sessions.Count} session(s) · Total {FormatDuration(TimeSpan.FromTicks(sessions.Sum(s => s.Duration.Ticks)))}[/]");
        AnsiConsole.Write(table);
    }

    private static string FormatDuration(TimeSpan duration) =>
        $"{(int)duration.TotalHours:00}:{duration.Minutes:00}:{duration.Seconds:00}";
}
