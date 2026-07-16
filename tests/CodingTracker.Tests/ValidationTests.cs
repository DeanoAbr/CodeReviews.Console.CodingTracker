using CodingTracker.Services;

namespace CodingTracker.Tests;

public sealed class ValidationTests
{
    [Theory]
    [InlineData("16-07-2026 09:30", true)]
    [InlineData("2026-07-16 09:30", false)]
    [InlineData("16/07/2026 09:30", false)]
    [InlineData("16-07-2026 9:30", false)]
    [InlineData("31-02-2026 09:30", false)]
    public void DateTimeInput_RequiresExactValidFormat(string input, bool expected) =>
        Assert.Equal(expected, Validation.TryParseDateTime(input, out _));

    [Fact]
    public void Range_RequiresEndAfterStart()
    {
        var start = new DateTime(2026, 7, 16, 10, 0, 0);
        Assert.True(Validation.IsValidRange(start, start.AddMinutes(1)));
        Assert.False(Validation.IsValidRange(start, start));
        Assert.False(Validation.IsValidRange(start, start.AddMinutes(-1)));
    }
}
