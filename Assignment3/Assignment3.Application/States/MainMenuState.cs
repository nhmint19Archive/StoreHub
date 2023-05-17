using Assignment3.Application.Services;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

internal class MainMenuState : AppState
{
    private readonly ConsoleService _consoleService;
    public MainMenuState(
        ConsoleService consoleService)
    {
        _consoleService = consoleService;
    }

    public override void Run()
    {
        var input = _consoleService.AskUserOption(
            new Dictionary<char, string>()
            {
                { 'S', "Sign In" },
                { 'B', "Browse our store" },
            },
            "Welcome to All Your Healthy Food Store!");

        switch (input)
        {
            case 'S':
                Console.WriteLine("Sign In Page");
                break;
            case 'B':
                OnStateChanged(this, nameof(BrowsingState));
                break;
        }
    }
}
