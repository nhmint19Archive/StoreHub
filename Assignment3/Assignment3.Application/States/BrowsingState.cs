using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Models;
using System.Linq.Expressions;

namespace Assignment3.Application.States;

/// <summary>
/// Allows customers to browse the store.
/// </summary>
internal class BrowsingState : AppState
{
    private readonly Catalogue _catalogue;
    private readonly UserSession _session;
    private readonly IConsoleView _view;
    private readonly IConsoleInputHandler _inputHandler;

    public BrowsingState(
        Catalogue catalogue,
        UserSession session,
        IConsoleView view,
        IConsoleInputHandler inputHandler)
    {
        _catalogue = catalogue;
        _session = session;
        _view = view;
        _inputHandler = inputHandler;
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
        var options = new Dictionary<char, string>()
        {
            { 'D', "Display Available Products" },
            { 'E', "Exit To Main Menu" },
            { 'O', "Manage Order" }
        };

        if (_catalogue.AreFiltersApplied)
        {
            options.Add('C', "Clear Filter");
        }
        else
        {
            options.Add('A', "Add Filter");
        }

        var input = _inputHandler.AskUserOption(options);

        switch (input)
        {
            case 'A':
                ShowFilters();
                break;
            case 'D':
                ShowProducts();
                break;
            case 'C':
                _catalogue.ResetFilters();
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
        var products = _catalogue.GetProducts();
        _view.Info($"Displaying {products.Count} available products:");

        foreach (var product in products)
        {
            _view.Info(string.Empty);
            _view.Info($"ID [{product.Id}] - Availability: {product.InventoryCount}");
            _view.Info($"{product.Name} - {product.Price} AUD");
            _view.Info($"{product.Description}");
        }
    }
    
    private void ShowSignedOutOptions()
    {
        var options = new Dictionary<char, string>()
        {
            { 'S', "Sign In To Start Shopping" },
            { 'E', "Exit to Main Menu" },
            { 'D', "Display Available Products" },
        };

        if (_catalogue.AreFiltersApplied)
        {
            options.Add('C', "Clear filter");
        }
        else
        {
            options.Add('A', "Add filter");
        }

        var input = _inputHandler.AskUserOption(options);

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
                _catalogue.ResetFilters();
                break;
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
        }
    }

    private void ShowFilters()
    {
       string? nameFilter;
        while (!_inputHandler.TryAskUserTextInput(
                    _ => true,
                    x => string.IsNullOrEmpty(x) ? null : x,
                    out nameFilter,
                    $"Please type the product name filter or press [{ConsoleKey.Enter}] if you don not want any filter"))
        {
        }

        if (nameFilter != null)
        {
            _catalogue.SetProductNameFilter(nameFilter);
        }
        else
        {
            _view.Info("No product name filters applied");
        }

        decimal upperPrice;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || (decimal.TryParse(x, out var y) && y >= 0),
                   x => string.IsNullOrEmpty(x) ?  decimal.MaxValue : decimal.Parse(x),
                   out upperPrice,
                   $"Please type the upper price limit or press [{ConsoleKey.Enter}] if you do not want one",
                   "Invalid input. Input must be empty or a positive number"))
        {
        }
        
        decimal lowerPrice;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || (decimal.TryParse(x, out var y) && y >= 0 && y <= upperPrice),
                   x => string.IsNullOrEmpty(x) ? default : decimal.Parse(x),
                   out lowerPrice,
                   $"Please type the lower price limit or press [{ConsoleKey.Enter}] if you do not want one",
                   $"Invalid input. Input must be empty or a positive number smaller than the upper price limit of ${upperPrice}"))
        {
        }
        
        if (upperPrice != decimal.MaxValue || lowerPrice != default)
        {
            _catalogue.SetPriceFilters(upperPrice, lowerPrice);
        } 
        else
        {
            _view.Info("No price filters applied");
        }
    }
}
