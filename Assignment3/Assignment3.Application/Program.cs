using Assignment3.Application.Controllers;
using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Application.States;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

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
        _ = services.AddScoped<ConsoleService>();
        _ = services.AddScoped<AppController>();
        _ = services.AddScoped<Catalogue>();
        _ = services.AddScoped<MainMenuState>();
        _ = services.AddScoped<BrowsingState>();
        _ = services.AddScoped<SignInState>();
        _ = services.AddScoped<IDictionary<Roles, UserAccount?>>();
        _ = services.AddScoped<UserSession>();
        _ = services.AddScoped<IReadOnlyDictionary<string, AppState>>(x =>
        {
            return new Dictionary<string, AppState>()
            {
                { nameof(MainMenuState), x.GetRequiredService<MainMenuState>() },
                { nameof(BrowsingState), x.GetRequiredService<BrowsingState>() },
                { nameof(SignInState), x.GetRequiredService<SignInState>() },
            };
        });

        // TODO: register objects here
        return services.BuildServiceProvider();
    }
}
