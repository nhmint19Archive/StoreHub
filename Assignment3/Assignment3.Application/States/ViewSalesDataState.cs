using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;

namespace Assignment3.Application.States;

/// <summary>
/// Allows a staff member or admin user to view sales statistics.
/// </summary>
internal class ViewSalesDataState : AppState
{
    private readonly SalesDataAnalyzer _salesDataAnalyzer;
    public ViewSalesDataState(
        UserSession session, 
        IConsoleView view, 
        IConsoleInputHandler inputHandler,
        SalesDataAnalyzer salesDataAnalyzer) : base(session, view, inputHandler)
    {
        _salesDataAnalyzer = salesDataAnalyzer;
    }

    /// <inheritdoc />
    public override void Run()
    {
        if (!_session.IsUserInRole(Roles.Admin) && !_session.IsUserInRole(Roles.Staff))
        {
            _view.Error("Invalid access to sales data page");
            _view.Info("Signing out");
            _session.SignOut();
            OnStateChanged(this, nameof(MainMenuState));
            return;
        }
        
        var input = _inputHandler.AskUserOption(new Dictionary<char, string>()
        {
            { 'E', "Exit to Main Menu" },
            { 'P', "Export Sales Data To CSV file" },
            { 'S', "Show Receipts" }
        });

        switch (input)
        {
            case 'P':
                PrintSalesData();
                break;
            case 'S':
                ShowReceipts();
                break;
            case 'E':
                OnStateChanged(this, nameof(MainMenuState));
                break;
        }
    }
    
    private void PrintSalesData()
    {
        var fileName = "sales_data.csv";
        if (_salesDataAnalyzer.TryWriteSalesDataToFile(fileName))
        {
            _view.Info($"Finished writing sales data to the file '{fileName}'");
            return;
        }
        
        _view.Error($"An error occurred whilst exporting sales data to '{fileName}'");
    }

    private void ShowReceipts()
    {
        using var context = new AppDbContext();
        var receipts = context.Receipts
            .Include(x => x.Order)
            .ThenInclude(x => x.Products)
            .ThenInclude(x => x.Product)
            .AsNoTracking()
            .OrderByDescending(x => x.Order.Date);

        foreach (var receipt in receipts)
        {
            _view.Info($"\nID [{receipt.Id}]");

            var totalPrice = 0m;
            foreach (var orderProduct in receipt.Order.Products)
            {
                _view.Info($"{orderProduct.Product.Name}-{orderProduct.ProductQuantity}");
                totalPrice += orderProduct.Product.Price * orderProduct.ProductQuantity;
            }
                
            _view.Info($"Total: ${totalPrice}");
            _view.Info($"Customer: {receipt.Order.CustomerEmail}");
            _view.Info(message: $"Time: {receipt.Order.Date.ToLocalTime()}");
        }
    }
}