namespace Assignment3.Application.Models;

public static class RegexPatterns
{
    public const string CommaSeparatedDigits = @"\d+,(\d+)*";
    public const string HyphenSeparatedDigits = $@"\d+-\d+";
    public const string DigitsOnly = @"^[0-9]+$";
    public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string Phone = @"^(\d{10})$";
    public const string CardNo = @"^(4|5|6)\d{3}-\d{4}-?\d{4}-\d{4}$";
    public const string Cvc = @"^\d{3}$";
    public const string StreetName =@"^[a-zA-Z0-9\s.'\-]+$";
    public const string Postal = @"^\d{4}$";
    public const string ApartmentNo = @"^[a-zA-Z0-9\s]+$";
}