using Assignment3.Application.Controllers;
using Assignment3.Application.Services;
using Assignment3.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment3.Application;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var services = RegisterDependencies();
        var appController = services.GetRequiredService<AppController>();
        using var scope = services.CreateScope();
        appController.Initialize();
    }

    private static ServiceProvider RegisterDependencies()
    {
        var services = new ServiceCollection();
        _ = services.AddScoped<ConsoleService>();
        _ = services.AddScoped<AppController>();
        _ = services.AddScoped<Catalogue>();

        // TODO: register objects here
        return services.BuildServiceProvider();
    }
}
