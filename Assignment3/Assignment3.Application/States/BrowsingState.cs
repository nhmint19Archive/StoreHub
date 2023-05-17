using Assignment3.Application.Services;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

internal class BrowsingState : AppState
{
    private readonly Catalogue _catalogue;
    private Func<Product, bool>? _priceFilter = null;
    private Func<Product, bool>? _nameFilter = null;

    public BrowsingState(
        Catalogue catalogue)
    {
        _catalogue = catalogue;
    }

    public override void Run()
    {
        ShowProducts();
        ShowOptions();
    }

    private void ShowProducts()
    {
        var products = _catalogue.GetProducts(_priceFilter, _nameFilter);
        Console.WriteLine($"Displaying {products.Count} available products: ");

        foreach (var product in products)
        {
            Console.WriteLine($"ID [{product.Id}] - Availability: {product.InventoryCount}");
            Console.WriteLine($"[{product.Name}] - [{product.Description}]-[{product.Price}]");
        }
    }

    private void ShowOptions()
    {
        var options = new Dictionary<char, string>()
        {
            { 'S', "Sign in to begin purchasing" },
            { 'E', "Exit to Main Menu" },
        };

        if (_nameFilter != null || _priceFilter != null)
        {
            options.Add('C', "Clear filter");
        }
        else
        {
            options.Add('A', "Add filter");
        }

        var input = ConsoleHelper.AskUserOption(options);

        switch (input)
        {
            case 'S':
                OnStateChanged(this, nameof(SignInState));
                break;
            case 'A':
                ShowFilters();
                break;
            case 'C':
                _priceFilter = null;
                _nameFilter = null;
                break;
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
        }
    }

    private void ShowFilters()
    {
        while (_nameFilter == null) {
            var productName = ConsoleHelper.AskUserTextInput("Please type the product name filter and press [Enter]");
            _nameFilter = p => p.Name.Contains(productName, StringComparison.InvariantCultureIgnoreCase);
        }

        while (_priceFilter == null) {
            var upperPrice = 0m;
            var upperPriceStr = ConsoleHelper.AskUserTextInput("Please type the upper price limit and press [Enter]");
            while (!Decimal.TryParse(upperPriceStr, out upperPrice)) {
                upperPriceStr = ConsoleHelper.AskUserTextInput("Please type in a valid number");
            }

            var lowerPrice = 0m;
            var lowerPriceStr = ConsoleHelper.AskUserTextInput("Please type the upper price limit and press [Enter]");
            while (!Decimal.TryParse(lowerPriceStr, out lowerPrice)) {
                lowerPriceStr = ConsoleHelper.AskUserTextInput("Please type in a valid number");
            }

            _priceFilter = p => p.Price <= upperPrice || p.Price >= lowerPrice;
        }

        return;
    }
}
