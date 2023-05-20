using Assignment3.Application.Controllers;
using Assignment3.Application.Models;
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
        using var scope = services.CreateScope();

        Product product1 = new Product { Id = 1, Name = "Product 1", Description = "This is the first product", Price = 10.5m, InventoryCount = 10 };
        Product product2 = new Product { Id = 2, Name = "Product 2", Description = "This is the second product", Price = 5.25m, InventoryCount = 20 };

        Dictionary<Product, uint> productQuantityDict1 = new Dictionary<Product, uint>();
        productQuantityDict1.Add(product1, 2);

        Dictionary<Product, uint> productQuantityDict2 = new Dictionary<Product, uint>();
        productQuantityDict2.Add(product2, 3);

        List<Dictionary<Product, uint>> productsList = new List<Dictionary<Product, uint>>();
        productsList.Add(productQuantityDict1);
        productsList.Add(productQuantityDict2);

        Order order = new Order(productsList);
        Transaction transaction = new Transaction();

        decimal totalPrice = 0;
        foreach (var productQuantityDict in productsList)
        {
            foreach (var kvp in productQuantityDict)
            {
                var product = kvp.Key;
                var quantity = kvp.Value;

                var productTotalPrice = product.Price * quantity;
                totalPrice += productTotalPrice;
            }
        }

        Invoice invoice = new Invoice(productsList, totalPrice);
        Receipt receipt = new Receipt(productsList, totalPrice, transaction.Id);

        appController.Run();
    }

    private static ServiceProvider RegisterDependencies()
    {
        var services = new ServiceCollection();
        _ = services.AddScoped<AppController>();
        _ = services.AddScoped<Catalogue>();
        _ = services.AddScoped<MainMenuState>();
        _ = services.AddScoped<BrowsingState>();
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
