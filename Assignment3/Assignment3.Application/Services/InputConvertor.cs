using System.Globalization;

namespace Assignment3.Application.Services;

/// <summary>
/// Convert raw user inputs into the specified format.
/// </summary>
internal static class InputConvertor
{
    /// <summary>
    /// Converts raw user input into a comma-separated integer list.
    /// </summary>
    /// <param name="input">Raw input string.</param>
    /// <returns>A collection of integers.</returns>
    /// <remarks>
    /// This method does not perform any validation.
    /// The string input is assumed to already be a comma-separated list of integers.
    /// For input validation, see <see cref="InputFormatValidator.ValidateCommaSeparatedNumberList(string)"/>.
    /// </remarks>
    public static IReadOnlyCollection<int> ToCommaSeparatedIntegerList(string input)
    {
        return string.IsNullOrEmpty(input)
            ? new List<int>()
            : input.Split(",")
                .Select(x => x.Trim())
                .Select(x => int.Parse(x))
                .ToList();
    }

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

    /// <summary>
    /// Converts raw user input into a decimal number.
    /// </summary>
    /// <param name="input">Raw input string.</param>
    /// <returns>A decimal number.</returns>
    /// <remarks>
    /// This method does not perform any validation.
    /// The string input is assumed to already be a valid decimal number.
    /// For input validation, see <see cref="InputFormatValidator.ValidateDecimal(string)"/>.
    /// </remarks>
    public static decimal ToDecimal(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return default;
        }

        return decimal.Parse(input, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts raw user input into an unsigned integer.
    /// </summary>
    /// <param name="input">Raw input string.</param>
    /// <returns>An unsigned integer.</returns>
    /// <remarks>
    /// This method does not perform any validation.
    /// The string input is assumed to already be a valid unsigned integer.
    /// For input validation, see <see cref="InputFormatValidator.ValidateUnsignedInteger(string)"/>.
    /// </remarks>
    public static uint ToUnsignedInteger(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return default;
        }

        return uint.Parse(input);
    }

    /// <summary>
    /// Converts raw user input into an integer.
    /// </summary>
    /// <param name="input">Raw input string.</param>
    /// <returns>An integer.</returns>
    /// <remarks>
    /// This method does not perform any validation.
    /// The string input is assumed to already be a valid integer.
    /// For input validation, see <see cref="InputFormatValidator.ValidateInteger(string)"/>.
    /// </remarks>
    public static int ToInteger(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return default;
        }

        return int.Parse(input);
    }
}
