namespace Assignment3.Application.Services;

/// <summary>
/// Convert raw user inputs into the specified format.
/// </summary>
internal static class InputConvertor
{
    /// <summary>
    /// Converts raw user input into a hyphen-separated integer pair.
    /// </summary>
    /// <param name="input">Raw input string.</param>
    /// <returns>A value tuple of 2 integers.</returns>
    /// <remarks>
    /// This method does not perform any validation.
    /// The string input is assumed to already be a hyphen-separated pair of integers.
    /// For input validation, see <see cref="InputFormatValidator.ValidateHyphenSeparatedNumberPair(string)"/>.
    /// </remarks>
    public static (int, int) ToHyphenSeparatedIntegerPair(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return (default, default);
        }

        var numberPair = input
                .Split("-")
                .Select(x => int.Parse(x))
                .ToList();

        return (numberPair.First(), numberPair.Last());
    }
}
