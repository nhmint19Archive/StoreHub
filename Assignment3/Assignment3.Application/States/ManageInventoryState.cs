using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Application.States
{
    internal class ManageInventoryState : AppState
    {
        private readonly Catalogue _catalogue;
        private readonly UserSession _session;
        private readonly IConsoleView _view;
        private readonly IConsoleInputHandler _inputHandler;

        public ManageInventoryState(Catalogue catalogue, UserSession session, IConsoleView view, IConsoleInputHandler inputHandler)
        {
            _catalogue = catalogue;
            _session = session;
            _view = view;
            _inputHandler = inputHandler;
        }

        public override void Run()
        {
            if (!_session.IsUserSignedIn)
            {
                _view.Error("Invalid access to ordering page");
                OnStateChanged(this, nameof(SignInState));
                return;
            }

            if (!_session.IsUserInRole(Roles.Staff))
            {
                _view.Error("Invalid access to ordering page");
                _view.Info("Signing out");
                _session.SignOut();
                OnStateChanged(this, nameof(MainMenuState));
                return;
            }

            bool exit = false;
            while (!exit)
            {
                ShowProducts();
                exit = SelectOption();
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

            var input = ConsoleHelper.AskUserOption(options);

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
            
            var name = ConsoleHelper.AskUserTextInput("Enter the name of the product");
            var description = ConsoleHelper.AskUserTextInput("Enter the description of the product");
            decimal price = 0;
            uint inventoryCount = 0;
            while (!_inputHandler.TryAskUserTextInput(
                   x => decimal.TryParse(x, out _),
                   x => decimal.Parse(x),
                   out price,
                   $"Please type the price of the product",
                   "Invalid input. Input must be empty or a valid number"))
            {}
            while (!_inputHandler.TryAskUserTextInput(
                   x => uint.TryParse(x, out _),
                   x => uint.Parse(x),
                   out inventoryCount,
                   $"Please type the quantity of the product",
                   "Invalid input. Input must be empty or a valid number"))
            {}
            
            using var context = new AppDbContext();
            context.Products.Add(new Product() { Name = name, Description = description, Price = price, InventoryCount = inventoryCount });
            context.SaveChanges();

        }
        private void UpdateProductPrice()
        {
            int id = 0;
            decimal price = 0;
            while (!_inputHandler.TryAskUserTextInput(
                   x => int.TryParse(x, out _),
                   x => int.Parse(x),
                   out id,
                   $"Please type the ID of the product",
                   "Invalid input. Input must be empty or a valid number"))
            { }
            while (!_inputHandler.TryAskUserTextInput(
                   x => decimal.TryParse(x, out _),
                   x => decimal.Parse(x),
                   out price,
                   $"Please type the price of the product",
                   "Invalid input. Input must be empty or a valid number"))
            { }

            using var context = new AppDbContext();
            var product = context.Products.Find(id);
            if (product != null)
            {
                product.Price = price;
                context.Products.Update(product);
                context.SaveChanges();
            }
            else
            {
                _view.Info("Could not find product with that ID.");
            }


        }
        private void UpdateProductQuantity()
        {
            int id = 0;
            uint inventoryCount = 0;
            while (!_inputHandler.TryAskUserTextInput(
                   x => int.TryParse(x, out _),
                   x => int.Parse(x),
                   out id,
                   $"Please type the ID of the product",
                   "Invalid input. Input must be empty or a valid number"))
            { }
            while (!_inputHandler.TryAskUserTextInput(
                   x => uint.TryParse(x, out _),
                   x => uint.Parse(x),
                   out inventoryCount,
                   $"Please type the quantity of the product",
                   "Invalid input. Input must be empty or a valid number"))
            { }

            using var context = new AppDbContext();
            var product = context.Products.Find(id);
            if (product != null)
            {
                product.InventoryCount = inventoryCount;
                context.Products.Update(product);
                context.SaveChanges();
            }
            else
            {
                _view.Info("Could not find product with that ID.");
            }
            

        }
        private void DeleteProduct()
        {
            int id = 0;
            while (!_inputHandler.TryAskUserTextInput(
                   x => int.TryParse(x, out _),
                   x => int.Parse(x),
                   out id,
                   $"Please type the ID of the product",
                   "Invalid input. Input must be empty or a valid number"))
            { }

            using var context = new AppDbContext();
            var product = context.Products.Find(id);
            if (product != null)
            {
                context.Products.Remove(product);
                context.SaveChanges();
            }
            else
            {
                _view.Info("Could not find product with that ID.");
            }

        }
    }
}
