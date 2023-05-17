using System.Text;

namespace Assignment3.Application.Services;

internal static class ConsoleHelper
{
    public static char AskUserOption(
        IReadOnlyDictionary<char, string> choices,
        string prompt = "Please select an option:")
    {
        PrintPrompt(prompt);
        foreach (var (choice, description) in choices)
        {
            Console.WriteLine($"[{char.ToUpper(choice)}] - {description}");
        }

        var input = Console.ReadLine();
        while (
            string.IsNullOrEmpty(input) ||
            input.Length != 1 ||
            !choices.ContainsKey(char.ToUpper(input.First())))
        {
            PrintPrompt("Please select a valid option");
            input = Console.ReadLine();
        }

        return char.ToUpper(input.First());
    }

    public static string AskUserTextInput(string prompt = "Please type your input:")
    {
        PrintPrompt(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    public static void PrintPrompt(string prompt)
    {
        Console.WriteLine($"> {prompt}");
    }

    public static void PrintError(string prompt)
    {
        Console.WriteLine($"! {prompt}");
    }
}
