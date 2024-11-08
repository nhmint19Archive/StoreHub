using System.Diagnostics.CodeAnalysis;

namespace StoreHub.Application.Services;

/// <summary>
/// Handle user input from the console.
/// </summary>
public interface IConsoleInputHandler
{
    /// <summary>
    /// Ask the user to select an option from the provided list.
    /// </summary>
    /// <param name="choices">List of choices and their descriptions.</param>
    /// <param name="prompt">Optional prompt.</param>
    /// <returns>The selected choice which is guaranteed to belong in the provided <paramref name="choices"/></returns>
    public char AskUserOption(
        IReadOnlyDictionary<char, string> choices,
        string prompt = "Please select an option:");

    /// <summary>
    /// Ask user for any text input.
    /// </summary>
    /// <param name="prompt">Optional prompt.</param>
    /// <returns>The raw input text or <c>string.Empty</c>.</returns>
    public string AskUserTextInput(string prompt = "Please type your input:");

    /// <summary>
    /// Ask user for a single key input.
    /// </summary>
    /// <param name="prompt">Optional prompt.</param>
    /// <returns>The pressed key.</returns>
    /// <remarks>
    /// This method is used to ask user to enter a single key and is intended to deal with special keys such as Escape or Enter.
    /// To ask user to choose from a list of choices, see <see cref="IConsoleInputHandler.AskUserOption(IReadOnlyDictionary{char, string}, string)"/>.
    /// </remarks>
    public ConsoleKey AskUserKeyInput(string prompt = "Please enter your key:");

    /// <summary>
    /// Ask user for text input and try converting it to a specified data type.
    /// </summary>
    /// <typeparam name="T">The output data type.</typeparam>
    /// <param name="validateFunc">The function that validates the input string.</param>
    /// <param name="convertFunc">The function that validates the output string to the type <typeparamref name="T"/>.</param>
    /// <param name="result">The converted result.</param>
    /// <param name="prompt">The optional prompt.</param>
    /// <param name="validationErrorMessage">The optional validation error message.</param>
    /// <param name="conversionErrorMessage">The optional conversion error message.</param>
    /// <returns><c>True</c> if the input is valid and successfully converted. Otherwise <c>False</c>.</returns>
    public bool TryAskUserTextInput<T>(
        Func<string, bool> validateFunc,
        Func<string, T> convertFunc,
        [MaybeNullWhen(false)] out T result,
        string prompt = "Please type your input:",
        string validationErrorMessage = "Invalid input",
        string conversionErrorMessage = "Failed to process input");
}