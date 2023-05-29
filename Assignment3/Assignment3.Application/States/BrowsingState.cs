using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using System.Linq.Expressions;

namespace Assignment3.Application.States;

/// <summary>
/// Allows customers to browse the store.
/// </summary>
internal class BrowsingState : AppState
{
    private readonly Catalogue _catalogue;

    public BrowsingState(
        Catalogue catalogue,
        UserSession session,
        IConsoleView view,
        IConsoleInputHandler inputHandler)  : base(session, view, inputHandler)
    {
        _catalogue = catalogue;
    }

    /// <inheritdoc />
    public override void Run()
    {
        var options = new Dictionary<char, string>()
        {
            { 'D', "Display Available Products" },
            { 'E', "Exit To Main Menu" },
        };

        if (_session.IsUserInRole(Roles.Customer))
        {
            options.Add('O', "Manage Order");
        }
        else
        {
            options.Add('S', "Sign in with a customer account to start ordering");
        }

        if (_catalogue.AreFiltersApplied)
        {
            options.Add('C', "Clear filter");
        }
        else
        {
            options.Add('A', "Add filter");
        }
        
        switch (_inputHandler.AskUserOption(options))
        {
            case 'A' when !_catalogue.AreFiltersApplied:
                AddFilters();
                break;
            case 'D':
                ShowProducts();
                break;
            case 'C' when _catalogue.AreFiltersApplied:
                _catalogue.ResetFilters();
                break;
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
            case 'O' when _session.IsUserInRole(Roles.Customer):
                OnStateChanged(this, nameof(OrderingState));
                break;
            case 'S' when !_session.IsUserSignedIn:
                OnStateChanged(this, nameof(SignInState));
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

    private void AddFilters()
    {
       string? nameFilter;
        while (!_inputHandler.TryAskUserTextInput(
                    _ => true,
                    x => string.IsNullOrEmpty(x) ? null : x,
                    out nameFilter,
                    $"Please type the product name filter. Type nothing and press [Enter] if you do not want to apply any name filter"))
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
                   $"Please type the upper price limit. Type nothing and press [Enter] if you do want to apply an upper price limit",
                   "Invalid input. Input must be empty or a positive number"))
        {
        }
        
        decimal lowerPrice;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || (decimal.TryParse(x, out var y) && y >= 0 && y <= upperPrice),
                   x => string.IsNullOrEmpty(x) ? default : decimal.Parse(x),
                   out lowerPrice,
                   $"Please type the lower price limit. Type nothing and press [Enter] if you do want to apply a lower price limit",
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
