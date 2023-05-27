using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Assignment3.Domain.Services;
using Microsoft.IdentityModel.Tokens;

namespace Assignment3.Application.States;

internal class AdminProfileState : AppState
{
    private readonly UserSession _session;  
    private readonly IConsoleView _view;
    private readonly IConsoleInputHandler _inputHandler;
    public AdminProfileState(
        UserSession session, 
        IConsoleInputHandler inputHandler, 
        IConsoleView view)
    {
        _session = session;
        _inputHandler = inputHandler;
        _view = view;
    }
    
    /// <inheritdoc/>
    public override void Run()
    {
        if (!_session.IsUserInRole(Roles.Admin))
        {
            _view.Error("Invalid access to admin page");
            _view.Info("Signing out");
            _session.SignOut();
            OnStateChanged(this, nameof(MainMenuState));
        }

        _view.Info("Admin Profile");

        var input = _inputHandler.AskUserOption(
            new Dictionary<char, string>()
            {
                { 'V', "View all staff accounts" },
                { 'A', "Alter a staff account" },
                { 'C', "Create a new staff account" },
                { 'E', "Exit to Main Menu" },
            });

        switch (input)
        {
            case 'V':
                ViewStaffAccounts();
                break;
            case 'A':
                ChangeStaffAccountDetails();
                break;
            case 'C':
                CreateStaffAccount();
                break;
            case 'S':
                ShowSalesData(); 
                break;
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
        }
    }

    private void CreateStaffAccount()
    {
        var email = _inputHandler.AskUserTextInput($"Enter the staff member's email. Press [{ConsoleKey.Enter}] to exit.");
        if (string.IsNullOrEmpty(email))
        {
            _view.Info("Empty email. No staff account created.");
            return;
        }
        
        var phone = _inputHandler.AskUserTextInput($"Enter the staff member's phone number. Press [{ConsoleKey.Enter}] to exit.");       
        if (string.IsNullOrEmpty(phone))
        {
            _view.Info("Empty phone number. No staff account created.");
            return;
        }
        
        var password = _inputHandler.AskUserTextInput($"Enter the account password. Press [{ConsoleKey.Enter}] to exit.");
        if (string.IsNullOrEmpty(password))
        {
            _view.Info("Empty password. No staff account created.");
            return;
        }

        var newStaffAccount = new StaffAccount()
        {
            Email = email,
            Phone = phone,
            Role = Roles.Staff,
        };
        
        newStaffAccount.SetPassword(password);
        var validationResults = ModelValidator.ValidateObject(newStaffAccount);
        if (validationResults.Count != 0)
        {
            _view.Errors(validationResults);
            return;
        }

        using var context = new AppDbContext();
        try
        {
            context.UserAccounts.Add(newStaffAccount);
            context.SaveChanges();
        }
        catch (Exception e) // TODO: catch more specific exception
        {
            _view.Error("Failed to register new staff account. Perhaps an account with this email already exists?");
#if DEBUG
            Console.WriteLine(e.Message);
#endif
            return;
        }

        _view.Info("Successfully created staff account");
    }

    private void ChangeStaffAccountDetails()
    {
        var email = _inputHandler.AskUserTextInput($"Enter the staff member's email. Press [{ConsoleKey.Enter}] to exit.");
        if (string.IsNullOrEmpty(email)) 
        {
            _view.Info("Empty email. No staff account updated.");
            return;
        }
        
        using var context = new AppDbContext();
        var staffAccount = context.UserAccounts.FirstOrDefault(x => x.Email == email && x.Role == Roles.Staff);
        if (staffAccount == null)
        {
            _view.Error($"Unable to find staff account with email '{email}'");
            return;
        }
        
        var newPhoneNumber = _inputHandler.AskUserTextInput($"Enter the staff member's new phone number. Press [{ConsoleKey.Enter}] if you do not want to change their phone number");
        var newPassword = _inputHandler.AskUserTextInput($"Enter the staff member's new password. Press [{ConsoleKey.Enter}] if you do not want to change their password");

        if (string.IsNullOrEmpty(newPhoneNumber) && string.IsNullOrEmpty(newPassword))
        {
            _view.Info("No details changed");
            return;
        }
        
        if (!string.IsNullOrEmpty(newPhoneNumber))
        {
            staffAccount.Phone = newPhoneNumber;
        } 
        
        if (!string.IsNullOrEmpty(newPassword))
        {
            staffAccount.SetPassword(newPassword);
        } 

        var validationResults = ModelValidator.ValidateObject(staffAccount);
        if (validationResults.Count != 0)
        {
            _view.Errors(validationResults);
            return;
        }

        try
        {
            context.UserAccounts.Update(staffAccount);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            _view.Error("Failed to change customer details.");
#if DEBUG
            Console.WriteLine(e.Message);
#endif
            return;
        }

        _view.Info("Successfully changed staff member details");
    }

    private void ViewStaffAccounts()
    {
        using var context = new AppDbContext();
        var staffAccounts = context
            .UserAccounts
            .Where(x => x.Role == Roles.Staff)
            .Select(x => new { x.Email, x.RegistryDate, x.Phone })
            .ToList();

        _view.Info($"Displaying {staffAccounts.Count} staff member(s)");
        foreach (var staff in staffAccounts)
        {
            _view.Info($"Email: {staff.Email} - Phone: {staff.Phone} - Registration Date: {staff.RegistryDate.ToLocalTime()}");
        }
    }

    private void ShowSalesData()
    {
        OnStateChanged(this, nameof(ViewSalesDataState));
    }
}