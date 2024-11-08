using StoreHub.Application.Models;
using StoreHub.Application.Services;

namespace StoreHub.Application.States;

/// <summary>
/// Acts the starting state of the application.
/// </summary>
internal class MainMenuState : AppState
{
    public MainMenuState(UserSession session, IConsoleView view, IConsoleInputHandler inputHandler)  : base(session, view, inputHandler)
    {
    }

    /// <inheritdoc />
    public override void Run()
    {
        var choices = new Dictionary<char, string>()
        {
            { 'B', "Browse our store" },
            { 'A', "Accounts" },
            { 'E', "Exit 'All Your Healthy Foods' Online Web Store" },
        };

        var input = _inputHandler.AskUserOption(
            choices,
            "Welcome to All Your Healthy Food Store!");

        switch (input)
        {
            case 'A':
                OnStateChanged(this, nameof(SignInState));
                break;
            case 'B':
                OnStateChanged(this, nameof(BrowsingState));
                break;
            case 'E':
                Environment.Exit(0);
                break;
        }
    }
}