using System.Globalization;
using CodingTracker.Models;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker.Data;

public sealed class CodingSessionRepository(string connectionString) : ICodingSessionRepository
{
    private const string StorageFormat = "yyyy-MM-dd HH:mm:ss.fffffff";

    public void InitializeDatabase()
    {
        using var connection = OpenConnection();
        connection.Execute("""
            CREATE TABLE IF NOT EXISTS CodingSessions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                StartTime TEXT NOT NULL,
                EndTime TEXT NOT NULL,
                CHECK (EndTime > StartTime)
            );
            """);
    }

    public int Create(CodingSession session)
    {
        using var connection = OpenConnection();
        return connection.QuerySingle<int>("""
            INSERT INTO CodingSessions (StartTime, EndTime)
            VALUES (@StartTime, @EndTime);
            SELECT last_insert_rowid();
            """, ToParameters(session));
    }

    public List<CodingSession> GetAll()
    {
        using var connection = OpenConnection();
        var rows = connection.Query<SessionRow>(
            "SELECT Id, StartTime, EndTime FROM CodingSessions;").ToList();
        return rows.Select(ToSession).ToList();
    }

    public bool Update(CodingSession session)
    {
        using var connection = OpenConnection();
        return connection.Execute("""
            UPDATE CodingSessions SET StartTime = @StartTime, EndTime = @EndTime
            WHERE Id = @Id;
            """, ToParameters(session)) == 1;
    }

    public bool Delete(int id)
    {
        using var connection = OpenConnection();
        return connection.Execute("DELETE FROM CodingSessions WHERE Id = @id;", new { id }) == 1;
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    private static object ToParameters(CodingSession session) => new
    {
        session.Id,
        StartTime = session.StartTime.ToString(StorageFormat, CultureInfo.InvariantCulture),
        EndTime = session.EndTime.ToString(StorageFormat, CultureInfo.InvariantCulture)
    };

    private static CodingSession ToSession(SessionRow row) => new()
    {
        Id = row.Id,
        StartTime = DateTime.ParseExact(row.StartTime, StorageFormat, CultureInfo.InvariantCulture),
        EndTime = DateTime.ParseExact(row.EndTime, StorageFormat, CultureInfo.InvariantCulture)
    };

    private sealed class SessionRow
    {
        public int Id { get; init; }
        public string StartTime { get; init; } = "";
        public string EndTime { get; init; } = "";
    }
}
