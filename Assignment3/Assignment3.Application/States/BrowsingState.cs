using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Models;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;

namespace Assignment3.Application.States;

internal class BrowsingState : AppState
{
    private readonly Catalogue _catalogue;
    private readonly UserSession _session;
    private Expression<Func<Product, bool>>? _priceFilter = null;
    private Expression<Func<Product, bool>>? _nameFilter = null;

    public BrowsingState(
        Catalogue catalogue,
        UserSession session)
    {
        _catalogue = catalogue;
        _session = session;
    }

    /// <inheritdoc />
    public override void Run()
    {
        if (_session.IsUserSignedIn)
        {
            ShowSignedInOptions();
        }
        else
        {
            ShowSignedOutOptions();
        }
    }

    private void ShowSignedInOptions()
    {
        // TODO: reduce duplication with ShowSignedOutOptions()
        var options = new Dictionary<char, string>()
        {
            { 'D', "Display Available Products" },
            { 'E', "Exit to Main Menu" },
            { 'O', "Add items to shopping cart" }
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
            case 'A':
                ShowFilters();
                break;
            case 'D':
                ShowProducts();
                break;
            case 'C':
                _priceFilter = null;
                _nameFilter = null;
                break;
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
            case 'O':
                OnStateChanged(this, nameof(OrderingState));
                break;
        }
    }

    private void ShowProducts()
    {
        var products = _catalogue.GetProducts(_priceFilter, _nameFilter);
        ConsoleHelper.PrintInfo($"Displaying {products.Count} available products:");

        foreach (var product in products)
        {
            ConsoleHelper.PrintInfo(string.Empty);
            ConsoleHelper.PrintInfo($"ID [{product.Id}] - Availability: {product.InventoryCount}");
            ConsoleHelper.PrintInfo($"{product.Name} - {product.Price} AUD");
            ConsoleHelper.PrintInfo($"{product.Description}");
        }
    }
    
    private void ShowSignedOutOptions()
    {
        var options = new Dictionary<char, string>()
        {
            { 'S', "Sign in to begin purchasing" },
            { 'E', "Exit to Main Menu" },
            { 'D', "Display Available Products" },
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
            case 'D':
                ShowProducts();
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
        while (!ConsoleHelper.TryAskUserTextInput(
                    x => true,
                    x => p => p.Name.Contains(x),
                    out _nameFilter,
                    "Please type the product name filter or press [Enter] if you don not want any filter"))
        {
        }

        var upperPrice = decimal.MaxValue;
        while (!ConsoleHelper.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || decimal.TryParse(x, out _),
                   x => string.IsNullOrEmpty(x) ? default : decimal.Parse(x),
                   out upperPrice,
                   "Please type the upper price limit or press [Enter] if you do not want one",
                   "Invalid input. Input must be empty or a valid number"))
        {
        }
        
        var lowerPrice = 0m;
        while (!ConsoleHelper.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || decimal.TryParse(x, out _),
                   x => string.IsNullOrEmpty(x) ? default : decimal.Parse(x),
                   out lowerPrice,
                   "Please type the lower price limit or press [Enter] if you do not want one",
                   "Invalid input. Input must be empty or a valid number"))
        {
        }
        
        _priceFilter = p => p.Price <= upperPrice && p.Price >= lowerPrice;
    }
}
