using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Enums;
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
        var choices = new Dictionary<char, string>()
        {
            { 'B', "Browse our store" },
            { 'A', "Accounts" },
        };

        var input = _consoleService.AskUserOption(
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
