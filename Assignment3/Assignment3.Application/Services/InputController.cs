using System.Text.RegularExpressions;

namespace Assignment3.Application.Services
{
    internal class InputController
    {
        public static bool ValidateCardNumber(string cardNumber)
        {
            // Check if the card number matches a valid credit card pattern
            Regex regex = new Regex(@"^(4|5|6)\d{3}[\ \-]?\d{4}[\ \-]?\d{4}[\ \-]?\d{4}$");
            if (!regex.IsMatch(cardNumber))
            {
                return false;
            }

            // Remove any spaces or dashes from the card number
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            // Apply the Luhn algorithm to validate the card number
            int sum = 0;
            int length = cardNumber.Length;
            for (int i = 0; i < length; i++)
            {
                int digit = int.Parse(cardNumber[i].ToString());

                if (i % 2 == length % 2)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }

                sum += digit;
            }

            return sum % 10 == 0;
        }
    }
}
