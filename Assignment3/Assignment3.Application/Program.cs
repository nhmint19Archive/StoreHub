using Assignment3.Application.Controllers;
using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Application.States;
using Assignment3.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

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
            .AddSingleton<IConsoleView, ConsoleView>()
            .AddSingleton<IConsoleInputHandler, ConsoleInputHandler>()
            .AddSingleton<AppController>()
            .AddSingleton<Catalogue>()
            .AddSingleton<MainMenuState>()
            .AddSingleton<BrowsingState>()
            .AddSingleton<OrderingState>()
            .AddSingleton<SignInState>()
            .AddSingleton<UserSession>()
            .AddSingleton<CustomerProfileState>()
            .AddSingleton<AdminProfileState>()
            .AddSingleton<StaffProfileState>()
            .AddSingleton<OrderingState>()
            .AddSingleton<ViewSalesDataState>()
            .AddSingleton<ManageInventoryState>()
            .AddSingleton<IReadOnlyDictionary<string, AppState>>(x => new Dictionary<string, AppState>()
            {
                { nameof(MainMenuState), x.GetRequiredService<MainMenuState>() },
                { nameof(BrowsingState), x.GetRequiredService<BrowsingState>() },
                { nameof(SignInState), x.GetRequiredService<SignInState>() },
                { nameof(CustomerProfileState), x.GetRequiredService<CustomerProfileState>() },
                { nameof(AdminProfileState), x.GetRequiredService<AdminProfileState>() },
                { nameof(StaffProfileState), x.GetRequiredService<StaffProfileState>() },
                { nameof(OrderingState), x.GetRequiredService<OrderingState>() },
                { nameof(ViewSalesDataState), x.GetRequiredService<ViewSalesDataState>() },
                { nameof(ManageInventoryState), x.GetRequiredService<ManageInventoryState>() }
            });


        return services.BuildServiceProvider();
    }
}
