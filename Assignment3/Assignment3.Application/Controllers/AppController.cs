using Assignment3.Application.Services;
using Assignment3.Application.States;
using Assignment3.Domain.Models;

namespace Assignment3.Application.Controllers;
internal class AppController
{
    private readonly ConsoleHelper _consoleService;
    private readonly Catalogue _catalogue;
    private readonly IReadOnlyDictionary<string, AppState> _appStates;
    private AppState _currentState;

    public AppController(
        ConsoleHelper consoleService,
        Catalogue catalogue,
        IReadOnlyDictionary<string, AppState> appStates)
    {
        _consoleService = consoleService;
        _catalogue = catalogue;
        _appStates = appStates;
        _currentState = _appStates[nameof(MainMenuState)];
        _currentState.StateChanged += SwitchState;
    }

    public void Run()
    {
        while (true)
        {
            _currentState.Run();
        }
    }

    private void SwitchState(object? sender, string newStateName)
    {
        _currentState.StateChanged -= SwitchState;
        _currentState = _appStates[newStateName];
        _currentState.StateChanged += SwitchState;
    }
}
