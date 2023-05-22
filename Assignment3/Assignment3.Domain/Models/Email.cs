namespace Assignment3.Domain.Models;

public class Email
{
    private readonly string _emailAddress;

    public Email(string emailAddress)
    {
        _emailAddress = emailAddress;
    }

    public void Send(string title, string content)
    {
        Console.WriteLine(title);
        Console.WriteLine(content);
    }
}