using System.Text.RegularExpressions;
using StoreHub.Application.Models;
using StoreHub.Application.Services;
using StoreHub.Domain.Data;
using StoreHub.Domain.Enums;
using StoreHub.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace StoreHub.Application.States;

/// <summary>
/// Allows a customer to manage their order.
/// </summary>
internal class OrderingState : AppState
{
    public OrderingState(
        UserSession session, 
        IConsoleView view, 
        IConsoleInputHandler inputHandler)  : base(session, view, inputHandler)
    {
    }

    /// <inheritdoc />
    public override void Run()
    {
        if (!_session.IsUserSignedIn)
        {
            _view.Error("Invalid access to ordering page");
            OnStateChanged(this, nameof(SignInState));
            return;
        }

        if (!_session.IsUserInRole(Roles.Customer))
        {
            _view.Error("Invalid access to ordering page");
            _view.Info("Signing out");
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
            choices.Add('D', "Delete existing order");
            choices.Add('C', "Confirm Order");
            choices.Add('V', "View Order");
        }
        else
        {
            choices.Add('A', "Add Order");
        }

        var input = _inputHandler.AskUserOption(choices);
        switch (input)
        {
            case 'A':
                AddOrUpdateProductsInOrder();
                break;
            case 'E' when order != null:
                RemoveProductsFromOrder(order.Id);
                break;
            case 'D' when order != null:
                DeleteExistingOrder(order.Id);
                break;
            case 'C' when order != null:
                ConfirmOrder(order);
                break;
            case 'B':
                OnStateChanged(this, nameof(BrowsingState));
                break;
            case 'V' when order != null:
                ViewOrder(order);
                break;
        }
    }

    private void ViewOrder(Order order)
    {
        _view.Info($"Pending order [{order.Id}]");
        _view.Info($"Creation date: {order.Date.ToLocalTime()}");
        _view.Info($"{order.Products.Count} Item(s):");
        foreach (var orderProduct in order.Products) 
        {
            _view.Info($"ID [{orderProduct.ProductId}] {orderProduct.Product.Name} - Quantity:  {orderProduct.ProductQuantity}");
        }
    }

    private void AddOrUpdateProductsInOrder(int? orderId = null)
    {
        var isOrderNew = orderId == null;
        using var context = new AppDbContext();
        Order? order;
        if (isOrderNew)
        {
            order = new Order(_session.AuthenticatedUser.Email);
        }
        else
        {
            order = context.Orders
                .Include(x => x.Products)
                .FirstOrDefault(x => x.Id == orderId);
            if (order == null)
            {
                order = new Order(_session.AuthenticatedUser.Email);
                isOrderNew = true;
            }
        }

        _view.Info($"Type the list of product ID - quantity pairs of items you'd like to purchase. Type [{ConsoleKey.Backspace}] when you are finished.");
        _view.Info($"For example: type '1-2 [{ConsoleKey.Enter}] 43-1 [{ConsoleKey.Backspace}]' to add 2 products with ID 1 and 1 product with ID 43");

        while (_inputHandler.AskUserKeyInput($"Press any key to continue. Press [{ConsoleKey.Backspace}] to exit.") != ConsoleKey.Backspace)
        {
            if (!_inputHandler.TryAskUserTextInput(
                    x => string.IsNullOrEmpty(x) || InputFormatValidator.ValidateHyphenSeparatedNumberPair(x),
                    x => string.IsNullOrEmpty(x)
                        ? (int.MinValue, int.MinValue)
                        : InputConvertor.ToHyphenSeparatedIntegerPair(x),
                    out var result,
                    $"Enter the product ID and quantity you'd like to add or update. Type nothing and press [Enter] to exit.",
                    "Input must be empty or a pair of hyphen-separated numbers"))
            {
                continue;
            }
            
            // the user is highly unlikely to enter this specific number so we can safely treat it as a special case
            if (result == (int.MinValue, int.MinValue))
            {
                break;
            }

            var (productId, productQuantity) = result;
            var isProductAlreadyAdded = false;
            foreach (var product in order.Products)
            {
                if (product.ProductId == productId && product.ProductQuantity != productQuantity)
                {
                    isProductAlreadyAdded = true;
                    product.ProductQuantity = productQuantity;
                    _view.Info(
                        $"Quantity of product ID [{product.ProductId}] changed to {product.ProductQuantity}");
                }
            }

            if (!isProductAlreadyAdded)
            {
                order.Products.Add(new OrderProduct
                {
                    ProductId = productId,
                    ProductQuantity = productQuantity,
                });
            }

            _view.Info($"Added {productQuantity} of product ID [{productId}]");
        }

        if (order.Products.Count == 0)
        {
            _view.Info("No products added to order");
            return;
        }

        var productIdList = order.Products.Select(x => x.ProductId).ToList();
        var products = context.Products
            .Where(x => productIdList.Contains(x.Id) && x.InventoryCount > 0)
            .Select(x => new { x.Id, x.InventoryCount })
            .ToDictionary(
                x => x.Id,
                x => x.InventoryCount);

        if (!ValidateOrderProductQuantity(order, products))
        {
            _view.Error("Ordered items are invalid");
            return;
        }
        
        if (isOrderNew)
        {
            _view.Info("Saving new order");
            context.Orders.Add(order);
            context.OrderProducts.AddRange(order.Products);       
        }
        else
        {
            _view.Info("Updating order");
            context.Orders.Update(order);
        }

        if (!context.TrySaveChanges())
        {
            _view.Error("Failed to process order");
            return;
        }
        
        _view.Info($"Successfully {(isOrderNew ? "added" : "updated")} order [{order.Id}]");
    }

    private bool ValidateOrderProductQuantity(Order order, IReadOnlyDictionary<int, int> availableProducts)
    {
        var errorMessages = new List<string>();
        IEnumerable<OrderProduct> validProducts = order.Products;

        var invalidProductIds = order.Products
            .Select(x => x.ProductId)
            .Where(x => !availableProducts.ContainsKey(x))
            .ToList();

        if (invalidProductIds.Count > 0)
        {
            validProducts = validProducts.ExceptBy(invalidProductIds, x => x.ProductId);
            errorMessages.Add($"The following product IDs do not exist: {string.Join(", ", invalidProductIds)}");
        }
        
        errorMessages.AddRange(
            validProducts
            .Where(x => x.ProductQuantity > availableProducts[x.ProductId])
            .Select(x => $"Invalid purchase quantity for product with ID [{x.ProductId}] (only {availableProducts[x.ProductId]} are available)")
            .ToList());

        if (errorMessages.Count <= 0)
        {
            return true;
        }
        
        _view.Errors(errorMessages);
        return false;
    }

    private Order? GetExistingOrderOrDefault()
    {
        using var context = new AppDbContext();
        return context.Orders
            .AsNoTracking()
            .Include(x => x.Products)
            .ThenInclude(x => x.Product)
            .Where(x => x.CustomerEmail == _session.AuthenticatedUser.Email && x.Status == OrderStatus.Unconfirmed)
            .OrderByDescending(x => x.Date)
            .FirstOrDefault();
    }

    private void DeleteExistingOrder(int orderId)
    {
        using var context = new AppDbContext();
        var order = context.Orders.Find(orderId);
        if (order == null)
        {
            return;
        }
        
        _view.Info($"Erasing order [{order.Id}]");
        context.Orders.Remove(order);
        context.SaveChanges();
    }
    
    private void RemoveProductsFromOrder(int orderId)
    {
        var productIdsToRemove = new HashSet<int>();
        while (_inputHandler.AskUserKeyInput($"Press any key to start entering IDs of products to remove. Press [{ConsoleKey.Backspace}] to exit.") != ConsoleKey.Backspace)
        {
            if (!_inputHandler.TryAskUserTextInput(
                    x => string.IsNullOrEmpty(x) || int.TryParse(x, out _),
                    x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                    out int? id,
                    $"Please type the ID of the product you would like to remove. Type nothing and press [Enter] to exit.",
                    "Invalid input. Input must be empty or a valid number"))
            {
                continue;
            }

            if (id == null)
            {
                break;
            }

            if (!productIdsToRemove.Contains(id.Value))
            {
                productIdsToRemove.Add(id.Value);
            }
        }

        if (productIdsToRemove.Count == 0)
        {
            _view.Info("No products removed from order");
        }
        else
        {
            _view.Info($"Removing the following products with IDs: {string.Join(", ", productIdsToRemove)}");
            using var context = new AppDbContext();
            var order = context.Orders
                .Include(x => x.Products)
                .FirstOrDefault(x => x.Id == orderId);
            
            if (order == null)
            {
                _view.Error($"Order [{orderId}] not found.");
                return;
            }

            var orderProductIds = order.Products.Select(x => x.ProductId).ToList();
            var invalidProductIdsToRemove = productIdsToRemove.Except(productIdsToRemove.Intersect(orderProductIds)).ToList();
            if (invalidProductIdsToRemove.Count > 0)
            {
                _view.Error($"The following product IDs cannot be removed because they are not in the order: {string.Join(",", invalidProductIdsToRemove)}");
                return;
            }

            order.Products = order.Products
                .Where(x => !productIdsToRemove.Contains(x.ProductId))
                .ToList();

            context.Orders.Update(order);
            if (!context.TrySaveChanges())
            {
                _view.Error("Failed to remove products from order");
                return;
            } 
        }

        AddOrUpdateProductsInOrder(orderId);
    }

    private void ConfirmOrder(Order order)
    {
        var deliveryMethod = AskUserForDeliveryMethod(order.Id);
        if (deliveryMethod == null)
        {
            _view.Info("No delivery method selected");
            return;
        }
        
        var transactionMethod = AskUserForPaymentMethod();
        if (transactionMethod == null)
        {
            _view.Info("No transaction method selected");
            return;
        }
        
        order.Finalize(deliveryMethod, transactionMethod);
        var success = order.Confirm();
        if (success)
        {
            _view.Info("Order successfully placed");
            return;
        }
        
        _view.Info("An error occurred whilst processing your order");
    }

    private IDeliveryMethod? AskUserForDeliveryMethod(int orderId)
    {
        var choice = _inputHandler.AskUserOption(new Dictionary<char, string>()
            {
                { 'P', "Pick up at store" },
                { 'D', "Postal delivery" },
                { 'E', "Exit" },
            },
            "Please select a delivery method");
        return choice switch
        {
            'P' => ProcessPickupMethod(orderId),
            'D' => ProcessPostalDelivery(orderId),
            'E' => null,
            _ => throw new InvalidOperationException(),
        };
    }

    private IDeliveryMethod? ProcessPostalDelivery(int orderId)
    {
        int? streetNumber;
        while (!_inputHandler.TryAskUserTextInput(
                   x => Regex.IsMatch(x, RegexPatterns.DigitsOnly) || string.IsNullOrEmpty(x),
                   x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                   out streetNumber,
                   $"Enter your address number. Type nothing and press [Enter] to cancel.",
                   "Street number is invalid."))
        { }
        
        if (streetNumber == null)
        {
            _view.Info("No street number entered.");
            return null;
        }
        
        string? streetName;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || Regex.IsMatch(x, RegexPatterns.StreetName),
                   x => x,
                   out streetName,
                   $"Enter your address street name. Type nothing and press [Enter] to cancel.",
                   "Street name is invalid.") )
        { }
        
        if (string.IsNullOrEmpty(streetName))
        {
            _view.Info("No street number entered.");
            return null;
        }
        
        int? postalCode;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || Regex.IsMatch(x, RegexPatterns.Postal),
                   x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                   out postalCode,
                   $"Enter your postal code. Type nothing and press [Enter] to cancel.",
                   "Postal code is invalid."))
        { }
        
        if (postalCode == null)
        {
            _view.Info("No post code entered.");
            return null;
        }

        string? apartmentNo;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || Regex.IsMatch(x, RegexPatterns.ApartmentNo),
                   x => string.IsNullOrEmpty(x) ? null : x,
                   out apartmentNo,
                   $"Enter your apartment number (if applicable)",
                   "Apartment number is invalid."))
        { }
        
        return new PostalDelivery(
            orderId,
            streetNumber.Value,
            streetName,
            postalCode.Value,
            apartmentNo ?? string.Empty,
            _session.AuthenticatedUser.Email);
    }

    private IDeliveryMethod ProcessPickupMethod(int orderId)
    {
        return new Pickup(orderId);
    }

    private ITransactionMethod? AskUserForPaymentMethod()
    {                                                
        var choice = _inputHandler.AskUserOption(new Dictionary<char, string>()          
            {                                                                            
                { 'P', "Paypal" },                                             
                { 'A', "Cash" },                   
                { 'B', "Bank Transfer" },  
                { 'C', "Credit Card" },             
                { 'E', "Exit"},
            },                                                                           
            "Please select a payment method");
        return choice switch
        {
            'P' => ProcessPaypalTransaction(),
            'A' => ProcessCashTransaction(),
            'B' => ProcessBankTransfer(),  
            'C' => ProcessCardTransaction(),    
            'E' => null,
            _ => throw new InvalidOperationException(),
        };
    }

    private ITransactionMethod? ProcessBankTransfer()
    {
        string? bsb;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || InputFormatValidator.ValidateBsb(x),
                   x => x,
                   out bsb,
                   $"Enter your BSB. Type nothing and press [Enter] to exit.",
                   "BSB is invalid."))
        { }

        if (string.IsNullOrEmpty(bsb))
        {
            _view.Info("No BSB entered.");
            return null;
        }
        
        string? accountNo;
        while (!_inputHandler.TryAskUserTextInput(
                   x =>  string.IsNullOrEmpty(x) || Regex.IsMatch(x, RegexPatterns.DigitsOnly),
                   x => x,
                   out accountNo,
                   "Enter your account number. Type nothing and press [Enter] to cancel",
                   "Account number is invalid."))
        { }

        if (string.IsNullOrEmpty(accountNo))
        {
            _view.Info("No account number entered.");
            return null;
        }

        return new BankTransaction(bsb, accountNo);
    }

    private ITransactionMethod? ProcessCardTransaction()
    {
        string? cardNo;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || InputFormatValidator.ValidateCardNumber(x),
                   x => x,
                   out cardNo,
                   "Enter your card number in the xxxx-xxxx-xxxx-xxxx format. Type nothing and press [Enter] to cancel.",
                   "Invalid input. Input must be empty or a valid card number"))
        { }

        if (string.IsNullOrEmpty(cardNo))
        {
            _view.Info("No card number entered.");
            return null;
        }
        
        string? cvc;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || Regex.IsMatch(x, RegexPatterns.Cvc),
                   x => x,
                   out cvc,
                   "Enter your card CVC. Type nothing and press [Enter] to cancel.",
                   "Invalid input. Input must be empty or a valid CVC"))
        { }

        if (string.IsNullOrEmpty(cvc))
        {
            _view.Info("No card CVC entered.");
            return null;
        }
        
        DateOnly? expiryDate;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || InputFormatValidator.ValidateCardExpiryDate(x),
                   x => string.IsNullOrEmpty(x) ? null : DateOnly.FromDateTime(DateTime.Parse(x)),
                   out expiryDate,
                   $"Enter your card expiry date in the mm/yyyy format. Type nothing and press [Enter] to cancel.",
                   "Invalid input. Input must be empty or a valid card expiry date"))
        { }

        if (expiryDate == null)
        {
            _view.Info("No card number entered.");
            return null;
        }
        
        return new CreditCardTransaction(cardNo, cvc, expiryDate.Value);
    }

    private ITransactionMethod? ProcessCashTransaction()
    {
        return new CashTransaction();
    }

    private ITransactionMethod? ProcessPaypalTransaction()
    {
        string? paypalUsername;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || 
                        Regex.IsMatch(x, RegexPatterns.Email) || 
                        Regex.IsMatch(x, RegexPatterns.Phone),
                   x => x,
                   out paypalUsername,
                   "Enter your PayPal email or phone number. Type nothing and press [Enter] to cancel.",
                   "PayPal username is invalid."))
        { }

        if ( string.IsNullOrEmpty(paypalUsername))
        {
            _view.Info("No Paypal identifier entered.");
            return null;
        }
        
        return new PaypalTransaction(paypalUsername);
    }
}
