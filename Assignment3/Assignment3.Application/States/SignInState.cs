using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Assignment3.Domain.Services;

namespace Assignment3.Application.States;

/// <summary>
/// Allows the user to sign in, create a new account or reset their password.
/// </summary>
internal class SignInState : AppState
{
    private readonly UserSession _session;  
    private readonly IConsoleView _view;
    private readonly IConsoleInputHandler _inputHandler;
    public SignInState(
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
        var userSignedIn = _session.IsUserSignedIn;
        if (!userSignedIn)
        {
            ShowSignedOutOptions();
        }
        else
        {
            ShowSignedInOptions();
        }
    }

    private void ShowSignedOutOptions()
    {
        var input = _inputHandler.AskUserOption(
            new Dictionary<char, string>()
            {
                { 'S', "Sign in with an existing account" },
                { 'C', "Create a new customer account" },
                { 'F', "Forgot password" },
                { 'E', "Exit to Main Menu" },
            });

        switch (input)
        {
            case 'S':
                SignIn();
                break;
            case 'C':
                CreateCustomerAccount();
                break;
            case 'F':
                ResetPassword();
                break;
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
        }
    }

    private void ResetPassword()
    {
        var email = _inputHandler.AskUserTextInput($"Enter the email of your account. Type nothing and press [Enter] to exit.");
        if (string.IsNullOrEmpty(email))
        {
            _view.Info("No account password reset");
            return;
        }

        using var context = new AppDbContext();
        var account = context.UserAccounts.Find(email);
        if (account == null)
        {
            _view.Error($"No account with the email '{email}' exists");
            return;
        }

        // pretend that the user receives and enters the correct reset code
        _ = _inputHandler.AskUserTextInput("Enter the reset code sent to your email");
        
        var newPassword = _inputHandler.AskUserTextInput("Enter your new password");
        account.SetPassword(newPassword);

        context.UserAccounts.Update(account);
        if (!context.TrySaveChanges())
        {
            _view.Error("Failed to change account password");
            return;
        }

        _session.SignIn(account);
        _view.Info("Successfully signed in");
    }

    private void ShowSignedInOptions()
    {
        var choices = new Dictionary<char, string>()
        {
            { 'S', "Sign Out"},
            { 'E', "Exit to Main Menu" },
        };

        var (prompt, newStateName) = _session.AuthenticatedUser.Role switch
        {
            Roles.Admin => ("View admin options", nameof(AdminProfileState)),
            Roles.Staff => ("View staff options", nameof(StaffProfileState)),
            Roles.Customer => ("View customer options", nameof(CustomerProfileState)),
            _ => throw new NotImplementedException(),
        };

        choices.Add('V', prompt);

        var input = _inputHandler.AskUserOption(choices);

        switch (input)
        {
            case 'S':
                _session.SignOut();
                _view.Info("Signed out successfully");
                break;
            case 'V':
                OnStateChanged(this, newStateName);
                break;               
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
        }
    }

    private void CreateCustomerAccount()
    {
        var email = _inputHandler.AskUserTextInput($"Choose your email. Type nothing and press [Enter] to exit.");
        if (string.IsNullOrEmpty(email))
        {
            _view.Info("No account created.");
            return;
        }
        
        var phone = _inputHandler.AskUserTextInput($"Choose your phone number. Type nothing and press [Enter] to exit.");
        if (string.IsNullOrEmpty(phone))
        {
            _view.Info("No account created.");
            return;
        }
        
        var password = _inputHandler.AskUserTextInput("Choose your password");
        var newUserAccount = new UserAccount()
        {
            Email = email,
            Phone = phone,
            Role = Roles.Customer,
        };
        
        newUserAccount.SetPassword(password);
        var validationResults = ModelValidator.ValidateObject(newUserAccount);
        if (validationResults.Count != 0)
        {
            _view.Errors(validationResults);
            return;
        }

        using var context = new AppDbContext();
        context.UserAccounts.Add(newUserAccount);
        if (!context.TrySaveChanges())
        {
            _view.Error("Failed to register new customer account. Perhaps an account with this email already exists?");
            return;
        }
        
        _session.SignIn(newUserAccount);
        _view.Info("Successfully signed in");
    }

    private void SignIn()
    {
        if (!_inputHandler.TryAskUserTextInput(
                x => !string.IsNullOrEmpty(x),
                x => x,
                out var email,
                "Enter account email",
                "Email must not be empty"))
        {
            return;
        }
        
        if (!_inputHandler.TryAskUserTextInput(
                x => !string.IsNullOrEmpty(x),
                x => x,
                out var password,
                "Enter account password",
                "Password must not be empty"))
        {
            return;
        }

        using var context = new AppDbContext();
        var userAccount = context.UserAccounts.FirstOrDefault(x => x.Email == email);
        if (userAccount == null)
        {
            _view.Error($"No account with the email '{email}' exists");
            return;
        }

        if (!userAccount.Authenticate(password))
        {
            _view.Error("Incorrect password");
            return;
        }

        _session.SignIn(userAccount);
        _view.Info("Successfully signed in");
    }
}
