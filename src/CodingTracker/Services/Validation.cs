using System.Globalization;

namespace CodingTracker.Services;

public static class Validation
{
    public const string DateTimeFormat = "dd-MM-yyyy HH:mm";

    public static bool TryParseDateTime(string? value, out DateTime result) =>
        DateTime.TryParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out result);

    public static bool IsValidRange(DateTime start, DateTime end) => end > start;

    public static bool TryParsePositiveId(string? value, out int id) =>
        int.TryParse(value, out id) && id > 0;
}
