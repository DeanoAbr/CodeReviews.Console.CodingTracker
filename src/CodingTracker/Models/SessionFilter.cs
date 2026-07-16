namespace CodingTracker.Models;

public enum Period { All, Day, Week, Month, Year }
public enum SortDirection { Ascending, Descending }

public sealed record SessionFilter(
    Period Period = Period.All,
    DateTime? ReferenceDate = null,
    SortDirection SortDirection = SortDirection.Descending);
