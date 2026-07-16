# Coding Tracker

A .NET 10 LTS console application for recording daily coding time. It supports manual entries, a live stopwatch, full CRUD operations, calendar-period filtering, and ascending or descending ordering.

## Features

- Add sessions using the exact `dd-MM-yyyy HH:mm` format
- Calculate duration automatically from start and end times
- Track a session as it happens with the stopwatch option
- View, update, and delete sessions
- Filter by day, Monday-to-Sunday week, month, or year
- Sort sessions by start time and see total duration
- Persist data in SQLite through Dapper
- Render menus and tables with Spectre.Console

## Run it

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), then run:

```powershell
dotnet restore
dotnet run --project src/CodingTracker
```

Run the tests with `dotnet test`.

The database is created automatically on first launch. Its path and SQLite connection string are controlled by `src/CodingTracker/appsettings.json`; a relative data source is resolved from the executable directory.

## Design and thought process

The application is split by responsibility: `Models` contains data, `Data` owns SQL and Dapper mapping, `Controllers` holds CRUD coordination and filtering, `Services` contains reusable validation and clock abstractions, and `UI` owns prompts and Spectre.Console tables.

SQLite has no dedicated date/time type, so timestamps are stored as invariant `yyyy-MM-dd HH:mm:ss.fffffff` text. That format sorts chronologically, preserves stopwatch precision, and is converted back into strongly typed `CodingSession` instances. The UI uses a separate, explicit human-facing format. `Duration` is calculated, making start and end times the single source of truth.

Filtering is deterministic, database-independent controller logic, so it can be unit tested without a real database. Repository access is hidden behind an interface for the same reason. The stopwatch uses an injected clock, keeping time acquisition separate from session creation.
