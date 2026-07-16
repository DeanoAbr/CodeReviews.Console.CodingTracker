using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace CodingTracker.Configuration;

public sealed class AppSettings
{
    public string DatabasePath { get; init; } = "Data/coding-tracker.db";
    public Dictionary<string, string> ConnectionStrings { get; init; } = [];

    public static AppSettings Load(string? settingsPath = null)
    {
        settingsPath ??= Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(settingsPath))
            throw new FileNotFoundException("The configuration file was not found.", settingsPath);

        var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(settingsPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (string.IsNullOrWhiteSpace(settings?.DatabasePath))
            throw new InvalidDataException("DatabasePath must be configured in appsettings.json.");

        return settings;
    }

    public string GetConnectionString()
    {
        var configured = ConnectionStrings.GetValueOrDefault("CodingTracker");
        var builder = string.IsNullOrWhiteSpace(configured)
            ? new SqliteConnectionStringBuilder { DataSource = DatabasePath }
            : new SqliteConnectionStringBuilder(configured);
        var path = Path.IsPathRooted(builder.DataSource)
            ? builder.DataSource
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, builder.DataSource));
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        builder.DataSource = path;
        return builder.ToString();
    }
}
