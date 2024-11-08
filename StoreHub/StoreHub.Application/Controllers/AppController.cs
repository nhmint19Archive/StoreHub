using StoreHub.Application.States;
using StoreHub.Domain.Data;

namespace StoreHub.Application.Controllers;

/// <summary>
/// Controls the flow of the application by switching between a set of states.
/// </summary>
internal class AppController
{
    private readonly IReadOnlyDictionary<string, AppState> _appStates;
    private AppState _currentState;

    public AppController(
        IReadOnlyDictionary<string, AppState> appStates)
    {
        _appStates = appStates;
        _currentState = _appStates[nameof(MainMenuState)];
        _currentState.StateChanged += SwitchState;

        EnsureDatabaseCreated();
        AppDbSeeder.SeedData();
    }

    private static void EnsureDatabaseCreated()
    {
        using var context = new AppDbContext();
        context.Database.EnsureCreated();
    }

    /// <summary>
    /// Run the application based on its current state.
    /// </summary>
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
