namespace Assignment3.Application.States;

/// <summary>
/// Represent a state that an application can be in.
/// Although all states use the same interface (the console), having different implementations
/// representing different states allow us to bind related behavior into a class, keeping
/// the states manageable and achieving high cohesion.
/// </summary>
internal abstract class AppState
{
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
