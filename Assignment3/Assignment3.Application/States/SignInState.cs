using Assignment3.Application.Services;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

internal class SignInState : AppState
{
    private readonly ConsoleService _consoleService;
    private readonly Catalogue _catalogue;
    public SignInState(
        ConsoleService consoleService,
        Catalogue catalogue)
    {
        _consoleService = consoleService;
        _catalogue = catalogue;
    }

    public override void Run()
    {
        var input = _consoleService.AskUserOption(
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
                ShowOptions();
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
        var input = _consoleService.AskUserOption(
            new Dictionary<char, string>()
            {
                { 'S', "Sign in to begin purchasing" },
                { 'F', "Add filter" },
                { 'E', "Exit" },
            });

        switch (input)
        {
            case 'S':
                // stop application
                break;
            case 'F':
                ShowFilters();
                break;
            case 'E':
                // stop application
                break;
        }
    }

    private void ShowFilters()
    {
        throw new NotImplementedException();
    }
}
