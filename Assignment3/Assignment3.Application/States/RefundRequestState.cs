using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

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
                { 'V', "View My Requests" },
                { 'R', "Request Refund" }
            };

            var input = _inputHandler.AskUserOption(options);

            switch (input)
            {
                case 'V':
                    ShowRequests();
                    break;
                case 'R':
                    RequestRefund();
                    break;
                case 'E':
                    OnStateChanged(this, nameof(CustomerProfileState));
                    break;
            }
        }

        private void ShowRequests()
        {
            using var context = new AppDbContext();
            var requests = context.RefundRequests
                .Where(x => x.Order.CustomerEmail == _session.AuthenticatedUser.Email)
                .Include(x => x.Order)
                .ThenInclude(x => x.Products)
                .ThenInclude(x => x.Product);

            if (requests.Any())
            {
                foreach (var request in requests)
                {
                    _view.Info($"Request for Order {request.OrderId}: ");
                    decimal totalPrice = 0;

                    foreach (var orderProduct in request.Order.Products)
                    {
                        _view.Info($"{orderProduct.Product.Name} - {orderProduct.ProductQuantity}"); 
                        totalPrice += orderProduct.Product.Price * orderProduct.ProductQuantity;
                    }

                    _view.Info($"Total: ${totalPrice}");
                    _view.Info($"Request Date: {request.Date}");
                    _view.Info($"Request Status: {request.RequestStatus}");
                }
            }
        }

        private void RequestRefund()
        {
            _view.Info($"Type the orderID you'd like to request a refund: ");
            var orderToRequest = 0;

            while (!int.TryParse(_inputHandler.AskUserTextInput(), out orderToRequest))
            {
                _view.Info("Invalid input, please type again: ");
            }

            using var context = new AppDbContext();
            var order = context.Orders.Find(orderToRequest);

            if (order == null)
            {
                _view.Error("Cannot find your order. Please do it again");
                return;
            }
            
            var requests = context.RefundRequests.Find(orderToRequest);

            if (requests != null)
            {
                _view.Error($"You already requested a refund for order [{orderToRequest}]");
                return;
            } 

            var description = _inputHandler.AskUserTextInput("Please provide description for your request: ");
            var request = new RefundRequest { Order = order, Description = description };

            context.RefundRequests.Add(request);
            if (!context.TrySaveChanges())
            {
                _view.Error("Failed to process refund request");
                return;
            }
            
            _view.Info("Refund has been sent successfully. Please wait for the store to process your request.");
            
            

        }
    }
}

