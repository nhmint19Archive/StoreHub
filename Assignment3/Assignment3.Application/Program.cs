using Assignment3.Application.Controllers;
using Assignment3.Application.Services;
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

        // TODO: register objects here
        return services.BuildServiceProvider();
    }
}
