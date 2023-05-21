using System.Linq;
using System.Text.RegularExpressions;
using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

internal class OrderingState : AppState
{
    private readonly UserSession _session;
    public OrderingState(UserSession session)
    {
        _session = session;
    }

    public override void Run()
    {
        if (!_session.IsUserSignedIn)
        {
            ConsoleHelper.PrintError("Invalid access to ordering page");
            OnStateChanged(this, nameof(SignInState));
            return;
        }

        if (!_session.IsUserInRole(Roles.Customer))
        {
            ConsoleHelper.PrintError("Invalid access to ordering page");
            ConsoleHelper.PrintInfo("Signing out");
            _session.SignOut();
            OnStateChanged(this, nameof(MainMenuState));
            return;
        }


        var choices = new Dictionary<char, string>()
        {
            { 'B', "Back to Browsing" },
        };

        var order = GetExistingOrderOrDefault();
        if (order != null)
        {
            choices.Add('E', "Edit Order");
            choices.Add('D', "Delete existing order and make a new one");
            choices.Add('C', "Confirm Order");

            var input = ConsoleHelper.AskUserOption(choices);
            switch (input)
            {
                case 'E':
                    // TODO: ask for product IDs that need to be removed
                    // ask for product IDs that need to be changed
                    EditOrder(order);
                    break;
                case 'D':
                    DeleteExistingOrder(order.Id);
                    AddProductsToShoppingCart();
                    break;
                case 'C':
                    ConfirmOrder(order);
                    break;
                case 'B':
                    OnStateChanged(this, nameof(BrowsingState));
                    break;
            }
        }
        else
        {
            choices.Add('A', "Add Order");
            var input = ConsoleHelper.AskUserOption(choices);
            switch (input)
            {
                case 'A':
                    AddProductsToShoppingCart();
                    break;
                case 'B':
                    OnStateChanged(this, nameof(BrowsingState));
                    break;
            }
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
                var numberPair = productQuantityStr
                    .Split("-")
                    .Select(x => int.Parse(x))
                    .ToList();
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

        var productIdList = order.Products.Select(x => x.ProductId).ToList();
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
    }

    private bool ValidateProducts(Order order, IReadOnlyDictionary<int, uint> availableProducts)
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

        var errorMessages = order
            .Products
            .Where(x => x.ProductQuantity > availableProducts[x.ProductId])
            .Select(x => $"Invalid purchase quantity for product with ID [{x.ProductId}] (only {availableProducts[x.ProductId]} are available")
            .ToList();

        if (errorMessages.Count > 0)
        {
            ConsoleHelper.PrintErrors(errorMessages);
            return false;
        }

        return true;
    }

    private Order? GetExistingOrderOrDefault()
    {
        using var context = new AppDbContext();
        return context.Orders
            .Where(x => x.CustomerEmail == _session.AuthenticatedUser.Email && x.Status == OrderStatus.Unconfirmed)
            .OrderByDescending(x => x.Date)
            .FirstOrDefault();
    }

    private void DeleteExistingOrder(int orderId)
    {
        using var context = new AppDbContext();
        var order = context.Orders.Find(orderId);
        if (order != null)
        {
            context.Orders.Remove(order);
            context.SaveChanges();
        }
    }

    private void EditOrder(Order order)
    {
        var productIdsToRemoveStr = ConsoleHelper.AskUserTextInput("Enter a comma separated list of IDs of products to be removed. Press [Enter] if you do not wish to remove any product");
        if (!string.IsNullOrEmpty(productIdsToRemoveStr) && !Regex.IsMatch(productIdsToRemoveStr, @"\d+,(\d+)*"))
        {
            ConsoleHelper.PrintError("Invalid input. Please type in a list of comma-separated product IDs");
            return;
        }

        var orderProductIds = order.Products.Select(x => x.ProductId).ToList();
        var productIdsToRemove = productIdsToRemoveStr
            .Split(",")
            .Select(x => x.Trim())
            .Select(x => int.Parse(x))
            .ToList();
        var invalidProductIdsToRemove = productIdsToRemove.Except(productIdsToRemove.Intersect(orderProductIds)).ToList();
        if (invalidProductIdsToRemove.Count > 0)
        {
            ConsoleHelper.PrintError($"The following product IDs cannot be removed because they are not in the order: {string.Join(",", invalidProductIdsToRemove)}");
            return;
        }

        ConsoleHelper.PrintInfo("Type the list of product ID - quantity pairs of items you'd like to purchase. Type [Esc] when you are finish.");
        ConsoleHelper.PrintInfo("For example: type '1-2 [Enter] 43-1 [Esc]' to add 2 products with ID 1 and 1 product with ID 43");
        var consoleKey = ConsoleKey.Enter;
        var productIdQuantityPairs = new Dictionary<int, int>();
        while (consoleKey != ConsoleKey.Escape)
        {
            var productQuantityStr = ConsoleHelper.AskUserTextInput("Enter the product ID and new quantity");
            if (!Regex.IsMatch(productQuantityStr, $@"\d+-\d+"))
            {
                ConsoleHelper.PrintError("Invalid input. Please see the input rules again");
            }
            else
            {
                var numberPair = productQuantityStr.Split("-").Select(x => int.Parse(x));
                productIdQuantityPairs.Add(numberPair.First(), numberPair.Last());
            }

            consoleKey = Console.ReadKey(false).Key;
        }

        var productIdsToUpdate = productIdQuantityPairs.Select(x => x.Key).ToList();
        var invalidProductIdsToUpdate = productIdsToUpdate.Except(productIdsToUpdate.Intersect(orderProductIds)).ToList();
        if (invalidProductIdsToUpdate.Count > 0)
        {
            ConsoleHelper.PrintError($"The following product IDs cannot be updated because they are not in the order: {string.Join(",", invalidProductIdsToUpdate)}");
            return;
        }

        foreach (var orderProduct in order.Products)
        {
            if (productIdQuantityPairs.TryGetValue(orderProduct.ProductId, out var updatedQuantity))
            {
                orderProduct.ProductQuantity = updatedQuantity;
            }
        }

        using var context = new AppDbContext();
        var productIdList = order.Products.Select(order => order.ProductId).ToList();
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

        try
        {
            context.Orders.Update(order);
            context.OrderProducts.UpdateRange(order.Products);
            context.SaveChanges();
        }
        catch
        {
            ConsoleHelper.PrintError("Failed to process order");
        }
    }

    private void ConfirmOrder(Order order)
    {
        var deliveryMethod = AskUserForDeliveryMethod();
        var transactionMethod = AskUserForPaymentMethod();
        var invoice = order.Prepare(deliveryMethod, transactionMethod);
        invoice.EmailToCustomer();
        var success = invoice.MakePayment();
        if (success)
        {
            ConsoleHelper.PrintInfo("Order successfully placed");
            order.StartDelivery();
            return;
        }
        
        ConsoleHelper.PrintError("An error occurred whilst processing your order");
    }

    private DeliveryMethod AskUserForDeliveryMethod()
    {
        throw new NotImplementedException();
    }
    private ITransactionStrategy AskUserForPaymentMethod()
    {                                                
        throw new NotImplementedException();         
    }                                                
}
