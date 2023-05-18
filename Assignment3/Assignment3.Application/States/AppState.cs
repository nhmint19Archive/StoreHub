namespace Assignment3.Application.States;

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
