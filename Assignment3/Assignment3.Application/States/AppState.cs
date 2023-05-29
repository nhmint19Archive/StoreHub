using Assignment3.Application.Models;
using Assignment3.Application.Services;

namespace Assignment3.Application.States;

/// <summary>
/// Represent a state that an application can be in.
/// Although all states use the same interface (the console), having different implementations
/// representing different states allow us to bind related behavior into a class, keeping
/// the states manageable and achieving high cohesion.
/// </summary>
internal abstract class AppState
{
    protected readonly UserSession _session;
    protected readonly IConsoleView _view;
    protected readonly IConsoleInputHandler _inputHandler;
    
    protected AppState(UserSession session, IConsoleView view, IConsoleInputHandler inputHandler)
    {
        _view = view;
        _inputHandler = inputHandler;
        _session = session;
    }

    /// <summary>
    /// Notifies that the application state is changed.
    /// </summary>
    public event EventHandler<string>? StateChanged;
    
    /// <summary>
    /// Run the current state.
    /// </summary>
    public abstract void Run();

    protected void OnStateChanged(AppState state, string newStateName)
    {
        StateChanged?.Invoke(state, newStateName);
    }
}
