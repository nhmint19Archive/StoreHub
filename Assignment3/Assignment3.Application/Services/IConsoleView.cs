namespace Assignment3.Application.Services;

/// <summary>
/// Print messages to the console.
/// </summary>
public interface IConsoleView
{
    /// <summary>
    /// Print an info message to the console.
    /// </summary>
    /// <param name="message">Info message.</param>
    void Info(string message);
    
    /// <summary>
    /// Print an error message to the console.
    /// </summary>
    /// <param name="error">Error message.</param>
    void Error(string error);    
    
    /// <summary>
    /// Print a list of errors to the console.
    /// </summary>
    /// <param name="errors">Error list.</param>
    void Errors(IReadOnlyCollection<string> errors);
}