﻿namespace Assignment3.Domain.Models;

internal static class EmailSimulator
{
    /// <summary>
    /// Simulates an email being sent.
    /// </summary>
    /// <param name="address">Email address of the recipient.</param>
    /// <param name="title">Email title.</param>
    /// <param name="content">Email content.</param>
    public static void Send(string address, string title, string content)
    {
        Console.WriteLine($"Simulated Email to <{address}>");
        Console.WriteLine(title);
        Console.WriteLine(content);
    }
}