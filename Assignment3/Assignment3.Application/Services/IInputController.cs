namespace Assignment3.Application.Services
{
    internal interface IInputController
    {
        bool ValidateInput(string input);
        string SanitiseInput(string input);
        bool ValidateEmail(string email);
        bool ValidateCardNumber(string cardNumber);
    }
}
