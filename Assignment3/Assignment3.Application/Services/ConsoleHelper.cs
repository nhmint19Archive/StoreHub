using System.Text;

namespace Assignment3.Application.Services;

internal class ConsoleHelper
{
    public char AskUserOption(
        IReadOnlyDictionary<char, string> choices,
        string prompt = "Please select an option:")
    {
        Console.WriteLine($"> {prompt}");
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
            Console.WriteLine("> Please select a valid option");
            input = Console.ReadLine();
        }

        return char.ToUpper(input.First());
    }

    public string AskUserTextInput(string prompt = "Please type your input:")
    {
        Console.WriteLine($"> {prompt}");
        return Console.ReadLine() ?? string.Empty;
    }
}
