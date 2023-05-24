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

        
        

        appController.Run();
    }

    private static ServiceProvider RegisterDependencies()
    {
        var services = new ServiceCollection();
        services
            .AddSingleton<AppController>()
            .AddSingleton<Catalogue>()
            .AddSingleton<MainMenuState>()
            .AddSingleton<BrowsingState>()
            .AddSingleton<SignInState>()
            .AddSingleton<UserSession>()
            .AddSingleton<CustomerProfileState>()
            .AddSingleton<AdminProfileState>()
            .AddSingleton<OrderingState>()
            .AddSingleton<IReadOnlyDictionary<string, AppState>>(x => new Dictionary<string, AppState>()
            {
                { nameof(MainMenuState), x.GetRequiredService<MainMenuState>() },
                { nameof(BrowsingState), x.GetRequiredService<BrowsingState>() },
                { nameof(SignInState), x.GetRequiredService<SignInState>() },
                { nameof(CustomerProfileState), x.GetRequiredService<CustomerProfileState>() },
                { nameof(AdminProfileState), x.GetRequiredService<AdminProfileState>() },
                { nameof(OrderingState), x.GetRequiredService<OrderingState>() }
            });


        return services.BuildServiceProvider();
    }
}
