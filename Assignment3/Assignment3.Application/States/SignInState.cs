using System.ComponentModel.DataAnnotations;
using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

internal class SignInState : AppState
{
    private readonly ConsoleService _consoleService;
    private readonly UserSession _currentSession;
    public SignInState(
        ConsoleService consoleService,
        UserSession currentSession)
    {
        _consoleService = consoleService;
        _currentSession = currentSession;
    }

    public override void Run()
    {
        var userSignedIn = _currentSession.IsUserSignedIn;
        if (!userSignedIn)
        {
            var input = _consoleService.AskUserOption(
                new Dictionary<char, string>()
                {
                    { 'S', "Sign in with an existing account" },
                    { 'C', "Create a new customer account" },
                },
                "User Profile");

            switch (input)
            {
                case 'S':
                    SignIn();
                    break;
                case 'C':
                    CreateCustomerAccount();
                    break;
            }

            return;
        }

        ShowSignedInOptions();
    }

    private void ShowSignedInOptions()
    {
        var choices = new Dictionary<char, string>();
        choices.Add('V', _currentSession.CurrentUser.Role switch
        {
            // TODO: jump to another state where staff account details can be changed/ created
            Roles.Admin => "View admin profile",
            // TODO: jump to another state where refund requests can be viewed and product inventory can be updated
            Roles.Staff => "View staff profile",
            // TODO: jump to another state where order history + logged refund requests can be seen
            Roles.Customer => "View customer profile",
            _ => throw new NotImplementedException(),
        });

        var input = _consoleService.AskUserOption(choices);

        switch (input)
        {
            case 'S':
                _currentSession.SignOut();
                break;
            case 'V':
                CreateCustomerAccount();
                break;
        }
    }

    private void CreateCustomerAccount()
    {
        var email = _consoleService.AskUserTextInput("Choose your email");
        var phone = _consoleService.AskUserTextInput("Choose your phone number");
        var password = _consoleService.AskUserTextInput("Choose your password");

        var newUserAccount = new CustomerAccount(password)
        {
            Email = email,
            Phone = phone,
            Role = Roles.Customer,
        };

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(newUserAccount, new ValidationContext(newUserAccount), validationResults))
        {
            Console.WriteLine("Invalid user details:");
            foreach (var error in validationResults.Select(x => x.ErrorMessage))
            {
                Console.WriteLine($"\t{error}");
            }

            return;
        }

        using var context = new AppDbContext();
        try
        {
            context.CustomerAccounts.Add(newUserAccount);
            context.SaveChanges();
        }
        catch (Exception) // TODO: catch more specific exception
        {
            Console.WriteLine("Failed to register new customer account. Perhaps an account with this email already exists?");
            return;
        }

        Console.WriteLine("Successfully signed in");
        _currentSession.SignIn(newUserAccount);
    }

    private void SignIn()
    {
        var email = _consoleService.AskUserTextInput("Enter username:");
        var password = _consoleService.AskUserTextInput("Enter password");
        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine("Email must not be empty");
            return;
        }
        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine("Password must not be empty");
            return;
        }

        using var context = new AppDbContext();
        var userAccount = context.CustomerAccounts.FirstOrDefault(x => x.Email == email);
        if (userAccount == null)
        {
            Console.WriteLine("User account does not exist");
            return;
        }

        if (!userAccount.Authenticate(password))
        {
            Console.WriteLine("Incorrect password");
            return;
        }

        _currentSession.SignIn(userAccount);
    }
}
