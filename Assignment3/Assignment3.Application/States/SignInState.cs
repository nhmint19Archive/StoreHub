using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

internal class SignInState : AppState
{
    private readonly UserSession _currentSession;
    public SignInState(
        UserSession currentSession)
    {
        _currentSession = currentSession;
    }

    public override void Run()
    {
        var userSignedIn = _currentSession.IsUserSignedIn;
        if (!userSignedIn)
        {
            var input = ConsoleHelper.AskUserOption(
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

        var input = ConsoleHelper.AskUserOption(choices);

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
        var email = ConsoleHelper.AskUserTextInput("Choose your email");
        var phone = ConsoleHelper.AskUserTextInput("Choose your phone number");
        var password = ConsoleHelper.AskUserTextInput("Choose your password");

        var newUserAccount = new CustomerAccount(password)
        {
            Email = email,
            Phone = phone,
            Role = Roles.Customer,
        };

        var validationResults = ValidationHelper.ValidateObject(newUserAccount);
        if (validationResults.Count != 0)
        {
            Console.WriteLine("Invalid user details:");
            foreach (var error in validationResults)
            {
                Console.WriteLine($"! {error}");
            }

            return;
        }

        using var context = new AppDbContext();
        try
        {
            context.UserAccounts.Add(newUserAccount);
            context.SaveChanges();
        }
        catch (Exception e) // TODO: catch more specific exception
        {
            Console.WriteLine("Failed to register new customer account. Perhaps an account with this email already exists?");
#if DEBUG
            Console.WriteLine(e.Message);
#endif
            return;
        }

        Console.WriteLine("Successfully signed in");
        _currentSession.SignIn(newUserAccount);
    }

    private void SignIn()
    {
        var email = ConsoleHelper.AskUserTextInput("Enter username:");
        var password = ConsoleHelper.AskUserTextInput("Enter password");
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
        var userAccount = context.UserAccounts.FirstOrDefault(x => x.Email == email);
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
