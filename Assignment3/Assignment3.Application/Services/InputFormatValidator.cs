using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Assignment3.Domain.Models;

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
    
    /// <summary>
    /// Validate that the input only contains digits
    /// </summary>
    /// <param name="input">Input string</param>
    /// <returns></returns>
    public static bool ValidateDigits(string input)
    {
        var regex = @"^[0-9]+$";
        return Regex.IsMatch(input, regex);
    }

    /// <summary>
    /// Validate that the input is email.                 
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns></returns>
    public static bool ValidateEmail(string input)
    {
        var regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(input, regex);
    }
    
    /// <summary>
    /// Validate that the input is phone number.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns></returns>
    
    public static bool ValidatePhone(string input)
    {
        var regex = @"^(\d{10})$";
        return Regex.IsMatch(input, regex);
    }
    /// <summary>
    /// Validate that the input is a valid card number.
    /// </summary>
    /// <param name="cardNumber">Input string.</param>
    /// <returns></returns>
    
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

    /// <summary>
    /// Validate that the input is a valid card CVC.
    /// </summary>
    /// <param name="cvc">Input string.</param>
    /// <returns></returns>
    public static bool ValidateCardCvc(string cvc)
    {
        // Check if the card CVC matches regex
        var regex = @"^\d{3}$";
        return Regex.IsMatch(cvc, regex);
    }

    /// <summary>
    /// Validate that the input is a valid card expiry date
    /// </summary>
    /// <param name="expiryDate">Input string.</param>
    /// <returns></returns>
    public static bool ValidateCardExpiryDate(string expiryDate)
    {
        string format = "MM/yy";

        // Check if the expiry date follows the format
        if (DateTime.TryParseExact(expiryDate, format, null, System.Globalization.DateTimeStyles.None,
                out DateTime parsedDate))
        {
            // Check if the parsed date is in the future / card not expired
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var expiryMonth = parsedDate.Month;
            var expiryYear = parsedDate.Year;
            if (expiryYear > currentYear || (expiryYear == currentYear && expiryMonth > currentMonth))
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Validate that the input is a valid Bank BSB.
    /// </summary>
    /// <param name="bsb">Input string</param>
    /// <returns></returns>
    public static bool ValidateBsb(string bsb)
    {
        // Check if the BSB has only digits
        if (!ValidateDigits(bsb))
            return false;
        
        // Convert to array
        bsb = new string(bsb.ToArray());

        // Check if the BSB has six digits
        if (bsb.Length != 6)
            return false;

        /*  The Australia BSB has 6 digits with this format
         *  First 2 numbers: bank code
         *  3rd number: state code
         *  3 last number: branch code
         **/
        // Extract bank, state, and branch codes
        var bankCode = bsb.Substring(0, 2);
        var stateCode = bsb.Substring(2, 1);
        var branchCode = bsb.Substring(3, 3);

        // Validate bank code (between 01 and 99)
        int bank;
        if (!int.TryParse(bankCode, out bank) || bank < 1 || bank > 99)
            return false;

        // Validate state code (between 0 and 9)
        int state;
        if (!int.TryParse(stateCode, out state) || state < 0 || state > 9)
            return false;

        // Validate branch code (between 001 and 999)
        int branch;
        if (!int.TryParse(branchCode, out branch) || branch < 1 || branch > 999)
            return false;
        return true;
    }

}
