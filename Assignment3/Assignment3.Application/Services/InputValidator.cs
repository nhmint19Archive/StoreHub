using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Assignment3.Application.Services;

public static class InputValidator
{
    public static bool ValidateCommaSeparatedNumberList(string input)
    {
        return string.IsNullOrEmpty(input) || Regex.IsMatch(input, @"\d+,(\d+)*");
    }

    public static bool ValidateHyphenSeparatedNumberPair(string inputStr)
    {
        return Regex.IsMatch(inputStr, $@"\d+-\d+") &&
            inputStr
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
