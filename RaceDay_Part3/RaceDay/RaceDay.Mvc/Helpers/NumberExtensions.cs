namespace RaceDay.Mvc;

/// <summary>
/// Small helper extensions used by the MVC views (e.g. ordinal finishing positions).
/// </summary>
public static class NumberExtensions
{
    /// <summary>
    /// Converts an integer to its English ordinal form.
    /// Examples: 1 → "1st", 2 → "2nd", 3 → "3rd", 11 → "11th", 47 → "47th".
    /// </summary>
    public static string ToOrdinal(this int number)
    {
        // Special case for the "teens" (11th, 12th, 13th)
        var lastTwo = Math.Abs(number) % 100;
        if (lastTwo is >= 11 and <= 13) return number + "th";

        return (Math.Abs(number) % 10) switch
        {
            1 => number + "st",
            2 => number + "nd",
            3 => number + "rd",
            _ => number + "th"
        };
    }
}
