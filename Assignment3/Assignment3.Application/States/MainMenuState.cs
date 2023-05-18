using Assignment3.Application.Services;

namespace Assignment3.Application.States;

internal class MainMenuState : AppState
{
    public MainMenuState()
    {
    }

    /// <inheritdoc />
    public override void Run()
    {
        var choices = new Dictionary<char, string>()
        {
            { 'B', "Browse our store" },
            { 'A', "Accounts" },
        };

        var input = ConsoleHelper.AskUserOption(
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
        }
    }
}
