using Assignment3.Application.Services;

namespace Assignment3.Application.States;

internal class MainMenuState : AppState
{
    private readonly ConsoleHelper _consoleHelper;
    public MainMenuState(
        ConsoleHelper consoleHelper)
    {
        _consoleHelper = consoleHelper;
    }

    public override void Run()
    {
        var choices = new Dictionary<char, string>()
        {
            { 'B', "Browse our store" },
            { 'A', "Accounts" },
        };

        var input = _consoleHelper.AskUserOption(
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
