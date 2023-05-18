using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;

namespace Assignment3.Application.States;

internal class CustomerProfileState : AppState
{
    private readonly UserSession _currentSession;
    public CustomerProfileState(UserSession currentSession)
    {
        _currentSession = currentSession;
    }

    /// <inheritdoc /> 
    public override void Run()
    {
        if (!_currentSession.IsUserSignedIn || _currentSession.CurrentUser.Role != Roles.Customer)
        {
            ConsoleHelper.PrintError("Invalid access to customer page");
            ConsoleHelper.PrintInfo("Signing out");
            _currentSession.SignOut();
            OnStateChanged(this, nameof(MainMenuState));
        }

        var choices = new Dictionary<char, string>()
        {
            { 'V', "View orders" },
            { 'C', "Change account details" },
            { 'E', "Exit to Main Menu" },
            { 'R', "Request refund" },
        };

        ConsoleHelper.PrintInfo("Customer Profile");
        ConsoleHelper.PrintInfo($"Email: {_currentSession.CurrentUser.Email}");
        ConsoleHelper.PrintInfo($"Phone: {_currentSession.CurrentUser.Phone}");
        ConsoleHelper.PrintInfo($"Registration Date: {_currentSession.CurrentUser.RegistryDate}");
        var input = ConsoleHelper.AskUserOption(choices);

        switch (input)
        {
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
            case 'V':
                ViewOrders();
                break;
            case 'C':
                ChangeAccountDetails();
                break;
            case 'R':
                RequestRefund();
                break;
        }        
    }

    private void ChangeAccountDetails()
    {
        var newPhoneNumber = ConsoleHelper.AskUserTextInput("Enter your new phone number or press enter if you do not want to change your phone number");
        var newPassword = ConsoleHelper.AskUserTextInput("Enter your new password or press enter if you do not want to change your password");

        if (string.IsNullOrEmpty(newPhoneNumber) && string.IsNullOrEmpty(newPassword))
        {
            ConsoleHelper.PrintInfo("No details changed");
            return;
        }

        using var context = new AppDbContext();
        var userAccount = context.UserAccounts.Find(_currentSession.CurrentUser.Email);
        if (userAccount == null)
        {
            ConsoleHelper.PrintError("Unable to find customer account");
            ConsoleHelper.PrintInfo("Signing out");
            _currentSession.SignOut();
            OnStateChanged(this, nameof(MainMenuState));
            return;
        }

        if (!string.IsNullOrEmpty(newPhoneNumber))
        {
            userAccount.Phone = newPhoneNumber;
        } 
        
        if (!string.IsNullOrEmpty(newPassword))
        {
            userAccount.SetPassword(newPassword);
        } 

        var validationResults = ValidationHelper.ValidateObject(userAccount);
        if (validationResults.Count != 0)
        {
            ConsoleHelper.PrintErrors(validationResults);
            return;
        }

        try
        {
            context.UserAccounts.Update(userAccount);
            context.SaveChanges();
        }
        catch (Exception e) // TODO: catch more specific exception
        {
            ConsoleHelper.PrintError("Failed to change customer details.");
#if DEBUG
            Console.WriteLine(e.Message);
#endif
            return;
        }

        ConsoleHelper.PrintInfo("Successfully changed customer details");
    }

    private void RequestRefund()
    {
        throw new NotImplementedException();
    }

    private void ViewOrders()
    {
        throw new NotImplementedException();
    }
}