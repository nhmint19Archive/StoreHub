using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Services;

namespace Assignment3.Application.States;

internal class CustomerProfileState : AppState
{
    private readonly UserSession _session;    
    private readonly IConsoleView _view;
    private readonly IConsoleInputHandler _inputHandler;
    public CustomerProfileState(
        UserSession session,        
        IConsoleView view,
        IConsoleInputHandler inputHandler)
    {
        _session = session;
        _view = view;
        _inputHandler = inputHandler;
    }

    /// <inheritdoc /> 
    public override void Run()
    {
        if (!_session.IsUserInRole(Roles.Customer))
        {
            _view.Error("Invalid access to customer page");
            _view.Info("Signing out");
            _session.SignOut();
            OnStateChanged(this, nameof(MainMenuState));
        }

        var choices = new Dictionary<char, string>()
        {
            { 'V', "View orders" },
            { 'C', "Change account details" },
            { 'E', "Exit to Main Menu" },
            { 'R', "Request refund" },
        };

        _view.Info("Customer Profile");
        _view.Info($"Email: {_session.AuthenticatedUser.Email}");
        _view.Info($"Phone: {_session.AuthenticatedUser.Phone}");
        _view.Info($"Registration Date: {_session.AuthenticatedUser.RegistryDate.ToLocalTime()}");
        var input = _inputHandler.AskUserOption(choices);

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
        var newPhoneNumber = _inputHandler.AskUserTextInput("Enter your new phone number or press enter if you do not want to change your phone number");
        var newPassword = _inputHandler.AskUserTextInput("Enter your new password or press enter if you do not want to change your password");

        // TODO(HUY): VALIDATE INPUT
        if (string.IsNullOrEmpty(newPhoneNumber) && string.IsNullOrEmpty(newPassword))
        {
            _view.Info("No details changed");
            return;
        }

        using var context = new AppDbContext();
        var userAccount = context.UserAccounts.Find(_session.AuthenticatedUser.Email);
        if (userAccount == null)
        {
            _view.Error("Unable to find customer account");
            _view.Info("Signing out");
            _session.SignOut();
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

        var validationResults = ModelValidator.ValidateObject(userAccount);
        if (validationResults.Count != 0)
        {
            _view.Errors(validationResults);
            return;
        }

        try
        {
            context.UserAccounts.Update(userAccount);
            context.SaveChanges();
        }
        catch (Exception e) // TODO: catch more specific exception
        {
            _view.Error("Failed to change customer details.");
#if DEBUG
            Console.WriteLine(e.Message);
#endif
            return;
        }

        _view.Info("Successfully changed customer details");
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