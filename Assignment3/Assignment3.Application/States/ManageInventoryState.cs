using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Assignment3.Domain.Services;

namespace Assignment3.Application.States
{
    /// <summary>
    /// Allows the staff member to update product inventory.
    /// </summary>
    internal class ManageInventoryState : AppState
    {
        private readonly Catalogue _catalogue;
        private readonly UserSession _session;
        private readonly IConsoleView _view;
        private readonly IConsoleInputHandler _inputHandler;

        public ManageInventoryState(
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
        
        /// <inheritdoc/>
        public override void Run()
        {
            if (!_session.IsUserSignedIn)
            {
                _view.Error("Invalid access to inventory management page");
                OnStateChanged(this, nameof(SignInState));
                return;
            }

            if (!_session.IsUserInRole(Roles.Staff) && !_session.IsUserInRole(Roles.Admin))
            {
                _view.Error("Invalid access to inventory management page");
                _view.Info("Signing out");
                _session.SignOut();
                OnStateChanged(this, nameof(MainMenuState));
                return;
            }
            
            // TODO: refactor this to make the behavior similar to BrowsingState for consistency
            var products = _catalogue.GetProducts();
            _view.Info($"Displaying {products.Count} available products:");
            foreach (var product in products)
            {
                ShowProduct(product);
            }

            while (!SelectOption())
            {}
        }

        private void ShowProduct(Product product)
        {
            _view.Info(string.Empty);
            _view.Info($"ID [{product.Id}] - Availability: {product.InventoryCount}");
            _view.Info($"{product.Name} - {product.Price} AUD");
            _view.Info($"{product.Description}");
        }

        private bool SelectOption()
        {
            var options = new Dictionary<char, string>()
                {
                    { 'C', "Add a New Product to the catalogue" },
                    { 'U', "Update a Products Price" },
                    { 'Q', "Update a Products Quantity" },
                    { 'D', "Delete a Product from the catalogue" },
                    { 'E', "Exit to Main Menu" }
                };

            var input = _inputHandler.AskUserOption(options);

            switch (input)
            {
                case 'C':
                    CreateProduct();
                    break;
                case 'U':
                    UpdateProductPrice();
                    break;
                case 'Q':
                    UpdateProductQuantity();
                    break;
                case 'D':
                    DeleteProduct();
                    break;
                case 'E':
                    OnStateChanged(this, nameof(MainMenuState));
                    return true;
            }
            return false;

        }

        private void CreateProduct()
        {
            var name = _inputHandler.AskUserTextInput($"Enter the name of the product. Press [{ConsoleKey.Enter}] to exit.");
            if (string.IsNullOrEmpty(name))
            {
                _view.Info("No product created.");
                return;
            }
            
            var description = _inputHandler.AskUserTextInput($"Enter the description of the product. Press [{ConsoleKey.Enter}] to exit.");
            if (string.IsNullOrEmpty(description))
            {
                _view.Info("No product created.");
                return;
            }
            
            decimal? price;
            while (!_inputHandler.TryAskUserTextInput(
                       x => string.IsNullOrEmpty(x) || decimal.TryParse(x, out _),
                       x => string.IsNullOrEmpty(x) ? null : decimal.Parse(x),
                       out price,
                       $"Please type the price of the product. Press [{ConsoleKey.Enter}] to exit.",
                       "Invalid input. Input must be empty or a valid number"))
            { }

            if (price == null)
            {
                _view.Info("No product created.");
                return;
            }
            
            int? inventoryCount;
            while (!_inputHandler.TryAskUserTextInput(
                       x => string.IsNullOrEmpty(x) || int.TryParse(x, out var y) && y >= 0,
                       x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                   out inventoryCount,
                   $"Please type the quantity of the product. Press [{ConsoleKey.Enter}] to exit.",
                   "Invalid input. Input must be empty or a valid number"))
            { }

            if (inventoryCount == null)
            {
                _view.Info("No product created.");
                return;
            }

            var product = new Product()
            {
                Name = name,
                Description = description,
                Price = price.Value,
                InventoryCount = inventoryCount.Value,
            };

            var errors = ModelValidator.ValidateObject(product);
            if (errors.Count > 0)
            {
                _view.Errors(errors);
                return;
            }
            
            using var context = new AppDbContext();
            context.Products.Add(product);
            if (!context.TrySaveChanges())
            {
                _view.Error("Failed to add product.");
                return;
            }
            
            _view.Info($"Product [{product.Id}] successfully added.");
            ShowProduct(product);
        }

        private void UpdateProductPrice()
        {
            int? id;
            while (!_inputHandler.TryAskUserTextInput(
                       x => string.IsNullOrEmpty(x) || int.TryParse(x, out _),
                       x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                       out id,
                       $"Please type the ID of the product. Press [{ConsoleKey.Enter}] to exit."))
            {
            }
            
            if (id == null)
            {
                _view.Info("No product updated");
                return;
            }
            
            using var context = new AppDbContext();
            var product = context.Products.Find(id);
            if (product == null)
            {
                _view.Error("Could not find product with that ID.");
                return;
            }
            
            _view.Info($"Product ID [{product.Id}] - {product.Name}");
            _view.Info($"Current price: ${product.Price}");
            
            decimal? price;
            while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || decimal.TryParse(x, out _),
                   x => string.IsNullOrEmpty(x) ? null : decimal.Parse(x),
                    out price,
                   $"Please type the price of the product. Press [{ConsoleKey.Enter}] to exit.",
                   "Invalid input. Input must be empty or a valid number"))
            { }
            
            if (price == null)
            {
                _view.Info("No product updated");
                return;
            }
            
            product.Price = price.Value;
            context.Products.Update(product);
            if (!context.TrySaveChanges())
            {
                _view.Error("Failed to update product.");
                return;
            }

            _view.Info($"Product [{id}] successfully updated.");
            ShowProduct(product);
        }
        
        private void UpdateProductQuantity()
        {
            int? id;
            while (!_inputHandler.TryAskUserTextInput(
                       x => string.IsNullOrEmpty(x) || int.TryParse(x, out _),
                       x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                       out id,
                       $"Please type the ID of the product. Press [{ConsoleKey.Enter}] to exit.",
                       "Invalid input. Input must be empty or a valid number"))
            { }
            
            if (id == null)
            {
                _view.Info("No product updated");
                return;
            }
            
            using var context = new AppDbContext();
            var product = context.Products.Find(id);
            if (product == null)
            {
                _view.Error("Could not find product with that ID.");
                return;
            }
            
            _view.Info($"Product ID [{product.Id}] - {product.Name}");
            _view.Info($"Current stock: {product.InventoryCount}");
            
            int? inventoryCount;
            while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || int.TryParse(x, out var y) && y >= 0,
                   x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                   out inventoryCount,
                   $"Please type the quantity of the product",
                   "Invalid input. Input must be a valid number"))
            { }
            
            if (inventoryCount == null)
            {
                _view.Info("No product updated");
                return;
            }
            
            product.InventoryCount = inventoryCount.Value;
            if (!context.TrySaveChanges())
            {
                _view.Error("Failed to update product.");
                return;
            }
            
            _view.Info($"Product [{id}] successfully updated.");
        }
        private void DeleteProduct()
        {
            int? id;
            while (!_inputHandler.TryAskUserTextInput(
                       x => string.IsNullOrEmpty(x) || int.TryParse(x, out _),
                       x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                       out id,
                       $"Please type the ID of the product. Press [{ConsoleKey.Enter}] to exit.",
                       "Invalid input. Input must be empty or a valid number"))
            { }

            if (id == null)
            {
                _view.Info("No product deleted");
                return;
            }
            
            using var context = new AppDbContext();
            var product = context.Products.Find(id);
            if (product == null)
            {
                _view.Error("Could not find product with that ID.");
                return;
            }

            context.Products.Remove(product);
            if (!context.TrySaveChanges())
            {
                _view.Error("Failed to delete product.");
                return;
            }
            
            _view.Info($"Product [{id}] successfully deleted.");
        }
    }
}
