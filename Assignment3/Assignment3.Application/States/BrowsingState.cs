using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Models;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

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
        ShowProducts();
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
            { 'E', "Exit to Main Menu" },
            { 'S', "Add items to shopping cart" }
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

    // TODO: move to OrderingState

    private void ShowSignedOutOptions()
    {
        var options = new Dictionary<char, string>()
        {
            { 'S', "Sign in to begin purchasing" },
            { 'P', "Print Sales Data" },
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
        // TODO(HUY): utilize ConsoleHelper.TryAskUserInput???
        while (_nameFilter == null) {
            var productName = ConsoleHelper.AskUserTextInput("Please type the product name filter and press [Enter]");
            _nameFilter = p => p.Name.Contains(productName);
        }

        while (_priceFilter == null) {
            var upperPrice = 0m;
            var upperPriceStr = ConsoleHelper.AskUserTextInput("Please type the upper price limit and press [Enter]");
            while (!decimal.TryParse(upperPriceStr, out upperPrice)) {
                ConsoleHelper.PrintError("Invalid input");
                upperPriceStr = ConsoleHelper.AskUserTextInput("Please type in a valid number");
            }

            var lowerPrice = 0m;
            var lowerPriceStr = ConsoleHelper.AskUserTextInput("Please type the upper price limit and press [Enter]");
            while (!decimal.TryParse(lowerPriceStr, out lowerPrice)) {
                ConsoleHelper.PrintError("Invalid input");
                lowerPriceStr = ConsoleHelper.AskUserTextInput("Please type in a valid number");
            }

            _priceFilter = p => p.Price <= upperPrice || p.Price >= lowerPrice;
        }
    }
}
