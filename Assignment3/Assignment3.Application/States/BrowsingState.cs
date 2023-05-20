using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Models;
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
            case 'S':
                this.AddProductsToShoppingCart();
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

    private void AddProductsToShoppingCart()
    {
        var order = new Order(_session.AuthenticatedUser.Email);
        ConsoleHelper.PrintInfo("Type the list of product ID - quantity pairs of items you'd like to purchase. Type [Esc] when you are finish.");
        ConsoleHelper.PrintInfo("For example: type '1-2 [Enter] 43-1 [Esc]' to add 2 products with ID 1 and 1 product with ID 43");
        var consoleKey = ConsoleKey.Enter;
        while (consoleKey != ConsoleKey.Escape)
        {
            var productQuantityStr = ConsoleHelper.AskUserTextInput("Enter the product ID and quantity");
            if (!Regex.IsMatch(productQuantityStr, $@"\d+-\d+"))
            {
                ConsoleHelper.PrintError("Invalid input. Please see the input rules again");
            }
            else
            {
                var numberPair = productQuantityStr.Split("-").Select(x => int.Parse(x));
                var productId = numberPair.First();
                var productQuantity = numberPair.Last();
                order.Products.Add(new OrderProduct
                {
                    ProductId = productId,
                    ProductQuantity = productQuantity,
                });
            }

            consoleKey = Console.ReadKey(false).Key;
        }

        var productIdList = order.Products.Select(order => order.ProductId).ToList();
        using var context = new AppDbContext();
        var products = context.Products
            .Where(x => productIdList.Contains(x.Id) && x.InventoryCount > 0)
            .Select(x => new { x.Id, x.InventoryCount })
            .ToDictionary(
                x => x.Id,
                x => x.InventoryCount);

        if (!ValidateProducts(order, products))
        {
            ConsoleHelper.PrintError("Ordered items are invalid");
            return;
        }

        // TODO: save order + associated order products
        try
        {
            context.Orders.Add(order);
            context.OrderProducts.AddRange(order.Products);
            context.SaveChanges();
        }
        catch
        {
            ConsoleHelper.PrintError("Failed to process order");
        }

        OnStateChanged(this, "PaymentState");

        static bool ValidateProducts(Order order, IReadOnlyDictionary<int, uint> availableProducts)
        {
            if (availableProducts.Count < order.Products.Count)
            {
                var invalidProductIds = order.Products
                    .Select(x => x.ProductId)
                    .Where(x => !availableProducts.ContainsKey(x))
                    .ToList();

                ConsoleHelper.PrintError($"The following product IDs are not valid: {string.Join(", ", invalidProductIds)}");
                return false;
            }

            var errorMessages = new List<string>();
            foreach (var orderProduct in order.Products)
            {
                 if (orderProduct.ProductQuantity > availableProducts[orderProduct.ProductId])
                {
                    errorMessages.Add($"Invalid purchase quantity for product with ID [{orderProduct.ProductId}] (only {availableProducts[orderProduct.ProductId]} are available");
                }
            }

            if (errorMessages.Count > 0)
            {
                ConsoleHelper.PrintErrors(errorMessages);
                return false;
            }

            return true;
        }
    }

    private void ShowSignedOutOptions()
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
