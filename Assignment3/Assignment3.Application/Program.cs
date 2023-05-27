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
        /*
         * The use of the Inversion of Control container (aka. Dependency Injection container)
         * greatly simplifies the startup process.
         */
        var services = RegisterDependencies();
        var appController = services.GetRequiredService<AppController>();
        appController.Run();
    }

    private static ServiceProvider RegisterDependencies()
    {
        /*
         * Dependency injection is a technique used to achieve inversion of control (IoC).
         * It decouples dependencies by providing them to the object, rather than having objects instantiate dependencies.
         * Testing also benefits from dependency injection. If we abstract dependencies behind an interface, during tests we can
         * develop mocks/stubs easily, enabling developers to test the target object without worrying about its dependencies.
         * Although not in the scope of this assignment, `IConsoleView` and `IConsoleInputHandler` are designed with this in mind.
         *
         * However, this means there still needs to be a component that handles the instantiation of a class and
         * construct objects in the correct order (for example if, component A depends on B, then B must be instantiated first).
         * Another complex issue would be the scope of created objects (singleton, scoped, etc.) but within this project,
         * the singleton scope is sufficient since objects are either designed to be so (`AppState` subclasses) or widely used by almost
         * every state (`IConsoleView` and `IConsoleInputHandler`).
         *
         * `ServiceCollection` is an implementation of the IoC container that addresses these issues.
         * It relieves developers from the manual construction of objects and managing their scope.
         * Instead, they can register objects and declare their required dependencies in the constructor.
         * The IoC container automatically resolves these dependencies at runtime.
         * Naturally, late-binding introduces the risk that unregistered dependencies may go unnoticed and result in runtime exceptions.
         * However, by conducting thorough code reviews and rigorous testing, the risk can be significantly mitigated.
         */
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
            .AddSingleton<ViewOrderState>()
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
                { nameof(ManageInventoryState), x.GetRequiredService<ManageInventoryState>() },
                { nameof(ViewOrderState), x.GetRequiredService<ViewOrderState>() }
            });

        return services.BuildServiceProvider();
    }
}
