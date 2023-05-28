using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Application.States;

/// <summary>
/// Allows a customer to create or view their refund requests.
/// </summary>
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
        
    /// <inheritdoc />
    public override void Run()
    {
        if (!_session.IsUserInRole(Roles.Customer))
        {
            _view.Error("Invalid access to customer page");
            _view.Info("Signing out");
            _session.SignOut();
            OnStateChanged(this, nameof(MainMenuState));
            return;
        }
            
        var options = new Dictionary<char, string>()
        {
            { 'E', "Exit to Main Menu" },
            { 'V', "View My Requests" },
            { 'S', "Show confirmed orders" },
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
            case 'S':
                ShowConfirmedOrder();
                break;
        }
    }

    private void ShowConfirmedOrder()
    {
        using var context = new AppDbContext();
        var orders = context.Orders
            .AsNoTracking()
            .Where(x => x.CustomerEmail == _session.AuthenticatedUser.Email && x.Status == OrderStatus.Confirmed)
            .Include(x => x.Products)
            .ThenInclude(x => x.Product)
            .OrderByDescending(x => x.Date);

        _view.Info("For the last week, you have paid for: ");
        foreach (var order in orders)
        {
            _view.Info(string.Empty);
            _view.Info($"[Order - {order.Id}]");

            var totalPrice = 0m;
            foreach (var orderProduct in order.Products)
            {
                _view.Info($"{orderProduct.Product.Name}-{orderProduct.ProductQuantity}");
                totalPrice += orderProduct.Product.Price * orderProduct.ProductQuantity;
            }

            _view.Info($"Total: ${totalPrice}");
            _view.Info(message: $"Time: {order.Date}");
        }
    }

    private void ShowRequests()
    {
        using var context = new AppDbContext();
        var requests = context.RefundRequests
            .Where(x => x.Order.CustomerEmail == _session.AuthenticatedUser.Email)
            .Include(x => x.Order)
            .ThenInclude(x => x.Products)
            .ThenInclude(x => x.Product)
            .ToList();

        if (requests.Count == 0)
        {
            _view.Info("No refund requests available");
            return;
        }
            
        foreach (var request in requests)
        {
            _view.Info($"Request for Order {request.OrderId}: ");
                
            var totalPrice = 0m;
            foreach (var orderProduct in request.Order.Products)
            {
                _view.Info($"{orderProduct.Product.Name} - {orderProduct.ProductQuantity}"); 
                totalPrice += orderProduct.Product.Price * orderProduct.ProductQuantity;
            }

            _view.Info($"Total: ${totalPrice}");
            _view.Info($"Request Date: {request.Date}");
            _view.Info($"Request Status: {request.RequestStatus}");
            _view.Info($"Comment from the store: {request.StaffComment}");
        }
    }

    private void RequestRefund()
    {
        int? orderToRefundId;
        while (!_inputHandler.TryAskUserTextInput(
                   x => string.IsNullOrEmpty(x) || int.TryParse(x, out _),
                   x => string.IsNullOrEmpty(x) ? null : int.Parse(x),
                   out orderToRefundId,
                   $"Type the order ID you'd like to request a refund. Press [{ConsoleKey.Enter}] if you do not want one",
                   "Invalid input. Input must be empty or a valid number"))
        { }

        if (orderToRefundId == null)
        {
            _view.Info("No refund requested.");
            return;
        }
            
        using var context = new AppDbContext();
        var order = context.Orders.Find(orderToRefundId);
        if (order == null)
        {
            _view.Error($"Order [{orderToRefundId}] not found.");
            return;
        }

        var requests = context.RefundRequests.Find(orderToRefundId);
        if (requests != null)
        {
            _view.Error($"You already requested a refund for order [{orderToRefundId}]");
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