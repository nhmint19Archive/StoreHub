using Assignment3.Application.Controllers;
using Assignment3.Application.Models;
using Assignment3.Application.States;
using Assignment3.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;

namespace Assignment3.Application;

internal class Program
{
    private static void Main(string[] args)
    {
        var services = RegisterDependencies();
        var appController = services.GetRequiredService<AppController>();
        using var scope = services.CreateScope();

        appController.Run();
    }

    private static ServiceProvider RegisterDependencies()
    {
        var services = new ServiceCollection();
        _ = services.AddScoped<AppController>();
        _ = services.AddScoped<Catalogue>();
        _ = services.AddScoped<MainMenuState>();
        _ = services.AddScoped<BrowsingState>();
        _ = services.AddScoped<ViewSalesDataState>();
        _ = services.AddScoped<SignInState>();
        _ = services.AddScoped<UserSession>();
        _ = services.AddScoped<CustomerProfileState>();
        _ = services.AddScoped<IReadOnlyDictionary<string, AppState>>(x => new Dictionary<string, AppState>()
        {
            { nameof(MainMenuState), x.GetRequiredService<MainMenuState>() },
            { nameof(BrowsingState), x.GetRequiredService<BrowsingState>() },
            { nameof(SignInState), x.GetRequiredService<SignInState>() },
            { nameof(CustomerProfileState), x.GetRequiredService<CustomerProfileState>() },
        });

        // TODO: register objects here
        return services.BuildServiceProvider();
    }
}
