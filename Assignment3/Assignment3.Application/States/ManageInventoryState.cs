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

            ShowProducts();
            SelectOption();
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

        private void SelectOption()
        {
            var options = new Dictionary<char, string>()
            {
                { 'C', "Add a New Product to the catalogue" },
                { 'U', "Update a Products Details" },
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
                    UpdateProduct();
                    break;
                case 'D':
                    DeleteProduct();
                    break;
                case 'E':
                    OnStateChanged(this, nameof(MainMenuState));
                    break;
            }
        }

        private void CreateProduct()
        {
            using var context = new AppDbContext();
            var Name = ConsoleHelper.AskUserTextInput("Enter the name of the product");
            var Description = ConsoleHelper.AskUserTextInput("Enter the description of the product");
            var Price = ConsoleHelper.AskUserTextInput("Enter the AUD price of the product");
            var InventoryCount = ConsoleHelper.AskUserTextInput("Enter the quantity of the product");

            context.Products.AddRange(new List<Product>()
        {
            new Product()
            {
                Name = Name,
                Description = Description,
                Price = Price,
                InventoryCount = InventoryCount
            }
        });
        }
        private void UpdateProduct()
        {
            using var context = new AppDbContext();
        }
        private void DeleteProduct()
        {
            using var context = new AppDbContext();
        }
    }
}
