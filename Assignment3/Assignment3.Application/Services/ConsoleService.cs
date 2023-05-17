using System.Text;

namespace Assignment3.Application.Services;

internal class ConsoleService
{
    public char AskForUserInput(
        IReadOnlyDictionary<char, string> choices,
        string prompt = "Please select an option:")
    {
        PrintSeparator();
        Console.WriteLine(prompt);
        foreach (var (choice, description) in choices)
        {
            Console.WriteLine($"[{char.ToUpper(choice)}] - {description}");
        }

        // TODO: validate against string inputs (only chars are allowed)
        var input = Console.ReadLine();
        while (
            string.IsNullOrEmpty(input) ||
            input.Length != 1 ||
            !choices.ContainsKey(char.ToLower(input.First())))
        {
            PrintSeparator();
            Console.WriteLine("Please select a valid option");
            input = Console.ReadLine();
        }

        return input.First();
    }

    private static void PrintSeparator(char separator = '#')
    {
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < 10; i++) {
            Console.Write(separator);
        }

        Console.WriteLine();
    }
}
