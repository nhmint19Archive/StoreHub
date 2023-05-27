namespace Assignment3.Application.Services;

public static class RegexConstants
{
    public const string CommaSeparatedRegex = @"\d+,(\d+)*";
    public const string HyphenSeparatedRegex = $@"\d+-\d+";
    public const string DigitsRegex = @"^[0-9]+$";
    public const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string PhoneRegex = @"^(\d{10})$";
    public const string CardNoRegex = @"^(4|5|6)\d{3}[\ \-]?\d{4}[\ \-]?\d{4}[\ \-]?\d{4}$";
    public const string CvcRegex = @"^\d{3}$";
    public const string StreetNameRegex =@"^[a-zA-Z0-9\s.'\-]+$";
    public const string PostalCodeRegex = @"^\d{4}$";
    public const string ApartmentNoRegex = @"^[a-zA-Z0-9\s]+$";
}