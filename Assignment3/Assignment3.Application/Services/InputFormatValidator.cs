using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Assignment3.Application.Services;

public static class InputFormatValidator
{
    /// <summary>
    /// Validate that the input is a comma-separated number list.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns><c>True</c> if the input is a comma-separated number list, otherwise <c>False</c>.</returns>
    public static bool ValidateCommaSeparatedNumberList(string input)
    {
        return string.IsNullOrEmpty(input) || 
            Regex.IsMatch(input, @"\d+,(\d+)*") &&
            input
                .Split(",")
                .Select(x => int.TryParse(x, out var r))
                .All(x => x);
    }

    /// <summary>
    /// Validate that the input is a hypen-separated pair of number.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns><c>True</c> if the input is a hypen-separated pair of number, otherwise <c>False</c>.</returns>
    public static bool ValidateHyphenSeparatedNumberPair(string input)
    {
        return Regex.IsMatch(input, $@"\d+-\d+") &&
            input
                .Split("-")
                .Select(x => int.TryParse(x, out var r))
                .All(x => x);
    }

    public static bool ValidateCardNumber(string cardNumber)
    {
        // Check if the card number matches a valid credit card pattern
        var regex = new Regex(@"^(4|5|6)\d{3}[\ \-]?\d{4}[\ \-]?\d{4}[\ \-]?\d{4}$");
        if (!regex.IsMatch(cardNumber))
        {
            return false;
        }

        // Remove any spaces or dashes from the card number
        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        // Apply the Luhn algorithm to validate the card number
        var sum = 0;
        var length = cardNumber.Length;
        for (var i = 0; i < length; i++)
        {
            var digit = int.Parse(cardNumber[i].ToString());

            if (i % 2 == length % 2)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
        }

        return sum % 10 == 0;
    }
}
