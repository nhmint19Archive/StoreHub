namespace Assignment3.Application.Services;

public class ConsoleView : IConsoleView
{    
    /// <inheritdoc/>
    public void Info(string message)
    {
        Console.WriteLine($">>> {message}");
    }
    
    /// <inheritdoc/>
    public void Error(string message)
    {
        Console.WriteLine($"!!! {message}");
    }
    
    /// <inheritdoc/>
    public void Errors(IReadOnlyCollection<string> errors)
    {
        Error("Error(s):");
        foreach (var error in errors)
        {
            Error(error);
        }
    }
}