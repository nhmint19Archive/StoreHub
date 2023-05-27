using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Application.States
{
    internal class ViewOrderState : AppState
    {
        private readonly UserSession _session;
        private readonly IConsoleView _view;
        private readonly IConsoleInputHandler _inputHandler;

        public ViewOrderState(
            UserSession session, IConsoleView view, IConsoleInputHandler inputHandler)
        {
            _session = session;
            _view = view;
            _inputHandler = inputHandler;
        }

        public override void Run()
        {
            if (!_session.IsUserInRole(Roles.Customer))
            {
                ConsoleHelper.PrintError("Invalid access to staff page");
                ConsoleHelper.PrintInfo("Signing out");
                _session.SignOut();
                OnStateChanged(this, nameof(MainMenuState));
            }

            ShowOrders();
            ShowDataOptions();
        }

        private void ShowDataOptions()
        {
            var options = new Dictionary<char, string>()
            {
                { 'E', "Exit to Main Menu" },
            };

            var input = _inputHandler.AskUserOption(options);

            switch (input)
            {
                case 'E':
                    OnStateChanged(this, nameof(MainMenuState));
                    break;
            }
        }

        private void ShowOrders()
        {
            using var context = new AppDbContext();
            var orders = context.Orders
                .Where(x => x.CustomerEmail == _session.AuthenticatedUser.Email)
                .Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .AsNoTracking()
                .OrderByDescending(x => x.Date);


            foreach (var order in orders)
            {
                _view.Info(string.Empty);
                _view.Info($"[Order - {order.Id}]");

                decimal totalPrice = 0;

                foreach (var orderProduct in order.Products)
                {
                    _view.Info($"{orderProduct.Product.Name}-{orderProduct.ProductQuantity}");
                    totalPrice += orderProduct.Product.Price * orderProduct.ProductQuantity;
                }

                _view.Info($"Total: ${totalPrice}");
                _view.Info(message: $"Time: {order.Date}");
                _view.Info($"Status: {order.Status}");
            }
        }
    }
}
