using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Application.States
{
    internal class RefundRequestState : AppState
    {
        private readonly UserSession _session;
        private readonly IConsoleView _view;
        private readonly IConsoleInputHandler _inputHandler;
        public RefundRequestState(
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

            ShowConfirmedOrder();
            ShowDataOptions();
        }

        private void ShowConfirmedOrder()
        {
            using var context = new AppDbContext();
            var orders = context.Orders
                .Where(x => x.CustomerEmail == _session.AuthenticatedUser.Email && x.Status == OrderStatus.Confirmed)
                .Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .AsNoTracking()
                .OrderByDescending(x => x.Date);

            _view.Info("For the last week, you have paid for: ");
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
            }
        }

        private void ShowDataOptions()
        {
            var options = new Dictionary<char, string>()
            {
                { 'E', "Exit to Main Menu" },
                { 'R', "Request Refund" }
            };

            var input = _inputHandler.AskUserOption(options);

            switch (input)
            {
                case 'R':
                    RequestRefund();
                    break;
                case 'E':
                    OnStateChanged(this, nameof(CustomerProfileState));
                    break;
            }
        }

        private void RequestRefund()
        {
            _view.Info($"Type the orderID you'd like to request a refund. Type [{ConsoleKey.Backspace}] to cancel");
            var orderToRequest = _inputHandler.AskUserTextInput("Order ID: ");


            try
            {
                using var context = new AppDbContext();
/*                var orders = context.Orders
                    .Where(x => x.Id == */

            } catch
            {
                _view.Error("Failed to process order");
            }






        }
    }
}
