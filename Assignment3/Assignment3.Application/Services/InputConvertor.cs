using System.Text.RegularExpressions;

namespace Assignment3.Application.Services;

internal static class InputConvertor
{
    public static IReadOnlyCollection<int> ToCommaSeparatedIntegerList(string input)
    {
        return input.Split(",")
            .Select(x => x.Trim())
            .Select(x => int.Parse(x))
            .ToList();
    }

    public static (int, int) ToHyphenSeparatedIntegerPair(string inputStr)
    {
        var numberPair = inputStr
                .Split("-")
                .Select(x => int.Parse(x))
                .ToList();

        return (numberPair.First(), numberPair.Last());
    }
}
