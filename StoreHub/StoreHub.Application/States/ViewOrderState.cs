using StoreHub.Application.Models;
using StoreHub.Application.Services;
using StoreHub.Domain.Data;
using StoreHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace StoreHub.Application.States;

/// <summary>
/// Allows a customer to view their order history.
/// Currently, there is only one option in this state, but it is expected to handle features such as
/// rating an order or  re-order it.
/// </summary>
internal class ViewOrderState : AppState
{
    public ViewOrderState(
        UserSession session, 
        IConsoleView view, 
        IConsoleInputHandler inputHandler) : base(session, view, inputHandler)
    {
    }

    /// <inheritdoc />
    public override void Run()
    {
        if (!_session.IsUserInRole(Roles.Customer))
        {
            _view.Error("Invalid access to staff page");
            _view.Info("Signing out");
            _session.SignOut();
            OnStateChanged(this, nameof(MainMenuState));
        }

        var options = new Dictionary<char, string>()
        {
            { 'E', "Exit to Main Menu" },
            { 'V', "View order history" }
        };

        var input = _inputHandler.AskUserOption(options);
        switch (input)
        {                
            case 'V':
                ShowOrders();
                break;
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

        if (orders.Any())
        {
            foreach (var order in orders)
            {
                _view.Info(string.Empty);
                _view.Info($"Order ID [{order.Id}]");

                var totalPrice = 0m;
                foreach (var orderProduct in order.Products)
                {
                    _view.Info($"{orderProduct.Product.Name}-{orderProduct.ProductQuantity}");
                    totalPrice += orderProduct.Product.Price * orderProduct.ProductQuantity;
                }

                _view.Info($"Total: ${totalPrice}");
                _view.Info($"Date: {order.Date.ToLocalTime()}");
                _view.Info($"Status: {order.Status}");
            }
        }
        else
        {
            _view.Info($"You have no orders");
        }
    }
}