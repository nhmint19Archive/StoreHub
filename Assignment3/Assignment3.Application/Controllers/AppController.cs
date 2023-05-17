using Assignment3.Application.Services;
using Assignment3.Domain.Models;

namespace Assignment3.Application.Controllers;
internal class AppController
{
    private readonly ConsoleService _consoleService;
    private readonly Catalogue _catalogue;
    public AppController(
        ConsoleService consoleService,
        Catalogue catalogue)
    {
        _consoleService = consoleService;
        _catalogue = catalogue;
    }

    public void Initialize()
    {
        var input = _consoleService.AskForUserInput(
            new Dictionary<char, string>()
            {
                { 'S', "Sign In" },
                { 'B', "Browse Our Store" },
            },
            "Welcome to All Your Healthy Food Store!");

        switch (input)
        {
            case 'S':
                Console.WriteLine("Sign In Page");
                break;
            case 'B':
                ShowProducts();
                break;
            default:
                throw new Exception();
        }
    }

    private void ShowProducts()
    {
        var products = _catalogue.GetProducts();
        Console.WriteLine($"Displaying {products.Count} available products: ");

        foreach (var product in products)
        {
            Console.WriteLine($"ID [{product.Id}] - Availability: {product.InventoryCount}");
            Console.WriteLine($"[{product.Name}]-[{product.Description}]-[{product.Price}]");
        }
    }

    private void ShowOptions()
    {
        var input = _consoleService.AskForUserInput(
            new Dictionary<char, string>()
            {
                { 'S', "Sign In To Begin Purchasing" },
                { 'E', "Exit" },
            });
    }
}
