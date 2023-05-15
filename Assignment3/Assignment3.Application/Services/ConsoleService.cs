namespace Assignment3.Application.Services;

internal class ConsoleService
{
    public ConsoleService()
    {
        Console.Write(Environment.NewLine);
    }

    public char AskForUserInput(IReadOnlyDictionary<char, string> choices)
    {
        Console.WriteLine("Please select an option:");
        foreach (var (choice, prompt) in choices)
        {
            Console.WriteLine($"[{choice}] - {prompt}");
        }

        // TODO: validate against string inputs (only chars are allowed)
        var input = Console.ReadKey().KeyChar;
        _ = Console.ReadLine();
        while (!choices.ContainsKey(input))
        {
            input = Console.ReadKey().KeyChar;
            _ = Console.ReadLine();
        }

        return input;
    }
}
