namespace Assignment3.Application.States;

internal abstract class AppState
{
    public event EventHandler<string>? StateChanged;

    public abstract void Run();

    protected void OnStateChanged(AppState state, string newStateName)
    {
        StateChanged?.Invoke(state, newStateName);
    }
}
