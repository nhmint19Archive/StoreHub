using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Assignment3.Application.Services;

internal static class ConsoleHelper
{
    /// <summary>
    /// Ask the user to select an option from the provided list.
    /// </summary>
    /// <param name="choices">List of choices and their descriptions.</param>
    /// <param name="prompt">Optional prompt.</param>
    /// <returns>The selected choice which is guaranteed to belong in the provided <paramref name="choices"/></returns>
    public static char AskUserOption(
        IReadOnlyDictionary<char, string> choices,
        string prompt = "Please select an option:")
    {
        PrintInfo(prompt);
        foreach (var (choice, description) in choices)
        {
            Console.WriteLine($"[{char.ToUpper(choice, CultureInfo.InvariantCulture)}] - {description}");
        }

        var input = Console.ReadLine();
        while (
            string.IsNullOrEmpty(input) ||
            input.Length != 1 ||
            !choices.ContainsKey(char.ToUpper(input.First(), CultureInfo.InvariantCulture)))
        {
            PrintInfo("Please select a valid option");
            input = Console.ReadLine();
        }

        return char.ToUpper(input.First(), CultureInfo.InvariantCulture);
    }
    
    /// <summary>
    /// Ask user for any text input.
    /// </summary>
    /// <param name="prompt">Optional prompt.</param>
    /// <returns>The raw input text or <c>string.Empty</c>.</returns>
    public static string AskUserTextInput(string prompt = "Please type your input:")
    {
        PrintInfo(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

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
    public static bool TryAskUserTextInput<T>(
        Func<string, bool> validateFunc,
        Func<string, T> convertFunc,
        [MaybeNullWhen(false)]
        out T result,
        string prompt = "Please type your input:",
        string validationErrorMessage = "Invalid input",
        string conversionErrorMessage = "Failed to process input")
    {
        var inputStr = AskUserTextInput(prompt);
        try
        {
            if (!validateFunc(inputStr))
            {
                ConsoleHelper.PrintError(validationErrorMessage);
                result = default;
                return false;
            }
        }
        catch
        {
            ConsoleHelper.PrintError(validationErrorMessage);
            result = default;
            return false;
        }

        try
        {
            result = convertFunc(inputStr);
            return true;
        }
        catch
        {
            ConsoleHelper.PrintError(conversionErrorMessage);
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Print an info message to the console.
    /// </summary>
    /// <param name="prompt">Prompt.</param>
    public static void PrintInfo(string prompt)
    {
        Console.WriteLine($">>> {prompt}");
    }

    /// <summary>
    /// Print an error message to the console.
    /// </summary>
    /// <param name="error">Error message.</param>
    public static void PrintError(string error)
    {
        Console.WriteLine($"!!! {error}");
    }

    /// <summary>
    /// Print a list of errors to the console.
    /// </summary>
    /// <param name="errors">Error list.</param>
    public static void PrintErrors(IEnumerable<string> errors)
    {
        ConsoleHelper.PrintError("Error(s):");
        foreach (var error in errors)
        {
            ConsoleHelper.PrintError(error);
        }
    }
}
