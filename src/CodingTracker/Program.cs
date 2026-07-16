using CodingTracker.Configuration;
using CodingTracker.Controllers;
using CodingTracker.Data;
using CodingTracker.Services;
using CodingTracker.UI;

try
{
    var settings = AppSettings.Load();
    var repository = new CodingSessionRepository(settings.GetConnectionString());
    repository.InitializeDatabase();

    var controller = new CodingController(repository);
    var userInput = new UserInput(controller, new SystemClock());
    userInput.Run();
}
catch (Exception exception)
{
    Spectre.Console.AnsiConsole.MarkupLine($"[red]Unable to start Coding Tracker:[/] {Spectre.Console.Markup.Escape(exception.Message)}");
    Environment.ExitCode = 1;
}
