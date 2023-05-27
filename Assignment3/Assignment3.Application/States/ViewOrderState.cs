using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Application.States
{
    /*ViewOrderState stays as seperate views so in the future we can add more options like "reorder" or "rating"*/
    /*For now, such options are not implemented so there's only option to Exit to Main Menu as right now*/

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
                _view.Error("Invalid access to staff page");
                _view.Info("Signing out");
                _session.SignOut();
                OnStateChanged(this, nameof(MainMenuState));
            }

            ShowOrders();
            ShowDataOptions();
        }

        private void ShowDataOptions()
        {
            /*More options to add in the future*/
            var options = new Dictionary<char, string>()
            {
                { 'E', "Exit to Main Menu" }
            };

            var input = _inputHandler.AskUserOption(options);

            switch (input)
            {
                case 'E':
                    OnStateChanged(this, nameof(CustomerProfileState));
                    break;
            }
        }

        private void ShowOrders()
        {
            using var context = new AppDbContext();
            var orders = context.Orders
                .AsNoTracking()
                .Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .Where(x => x.CustomerEmail == _session.AuthenticatedUser.Email)
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
                _view.Info($"Time: {order.Date}");
                _view.Info($"Status: {order.Status}");
            }
        }
    }
}
