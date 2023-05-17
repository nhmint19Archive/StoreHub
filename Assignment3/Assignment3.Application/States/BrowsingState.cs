using Assignment3.Application.Services;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

internal class BrowsingState : AppState
{
    private readonly ConsoleService _consoleService;
    private readonly Catalogue _catalogue;
    public BrowsingState(
        ConsoleService consoleService,
        Catalogue catalogue)
    {
        _consoleService = consoleService;
        _catalogue = catalogue;
    }


    public override void Run()
    {
        ShowProducts();
        ShowOptions();
    }

    private void ShowProducts()
    {
        var products = _catalogue.GetProducts();
        Console.WriteLine($"Displaying {products.Count} available products: ");

        foreach (var product in products)
        {
            Console.WriteLine($"ID [{product.Id}] - Availability: {product.InventoryCount}");
            Console.WriteLine($"[{product.Name}] - [{product.Description}]-[{product.Price}]");
        }
    }

    private void ShowOptions()
    {
        var input = _consoleService.AskUserOption(
            new Dictionary<char, string>()
            {
                { 'S', "Sign in to begin purchasing" },
                { 'F', "Add filter" },
                { 'E', "Exit to Main Menu" },
            });

        switch (input)
        {
            case 'S':
                OnStateChanged(this, nameof(SignInState));
                break;
            case 'F':
                ShowFilters();
                break;
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
        }
    }

    private void ShowFilters()
    {
        // TODO: handle filters
    }
}
