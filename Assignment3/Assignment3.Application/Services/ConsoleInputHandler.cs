using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Assignment3.Application.Services;

public class ConsoleInputHandler : IConsoleInputHandler
{
    private readonly IConsoleView _view;

    public ConsoleInputHandler(IConsoleView view)
    {
        _view = view;
    }

    /// <inheritdoc/>
    public char AskUserOption(
        IReadOnlyDictionary<char, string> choices,
        string prompt = "Please select an option:")
    {
        _view.Info(prompt);
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
            _view.Error("Please select a valid option");
            input = Console.ReadLine();
        }

        return char.ToUpper(input.First(), CultureInfo.InvariantCulture);
    }
    
    /// <inheritdoc/>
    public string AskUserTextInput(string prompt = "Please type your input:")
    {
        _view.Info(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    /// <inheritdoc/>
    public ConsoleKey AskUserKeyInput(string prompt = "Please enter your key:")
    {
        _view.Info(prompt);
        var result = Console.ReadKey(true).Key;
        return result;
    }

    /// <inheritdoc/>
    public bool TryAskUserTextInput<T>(
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
                _view.Error(validationErrorMessage);
                result = default;
                return false;
            }
        }
        catch
        {
            _view.Error(validationErrorMessage);
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
            _view.Error(conversionErrorMessage);
            result = default;
            return false;
        }
    }
}