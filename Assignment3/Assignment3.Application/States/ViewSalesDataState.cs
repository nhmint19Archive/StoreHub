using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using Assignment3.Domain.Enums;

namespace Assignment3.Application.States;

/// <summary>
/// Allows a staff member or admin user to view sales statistics.
/// </summary>
internal class ViewSalesDataState : AppState
{
    private readonly UserSession _session;
    private readonly IConsoleView _view;
    private readonly IConsoleInputHandler _inputHandler;
    public ViewSalesDataState(
        UserSession session, 
        IConsoleView view, 
        IConsoleInputHandler inputHandler)
    {
        _session = session;
        _view = view;
        _inputHandler = inputHandler;
    }

    /// <inheritdoc />
    public override void Run()
    {
        if (!_session.IsUserInRole(Roles.Admin) || !_session.IsUserInRole(Roles.Staff))
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
            { 'P', "Print Sales Data" },
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
        try
        {            
            using var context = new AppDbContext();
            var receipts = context.Receipts
                .Include(x => x.Order)
                .ThenInclude(x => x.Products)
                .ThenInclude(x => x.Product)
                .AsNoTracking()
                .AsEnumerable();

            var csvData = new List<string[]>();

            foreach (var receipt in receipts)
            {
                var totalPrice = receipt.Order.Products.Sum(orderProduct => orderProduct.Product.Price * orderProduct.ProductQuantity);
                var stringData = new string[] { 
                    $"{receipt.Id}", 
                    $"{receipt.OrderId}", 
                    $"{receipt.Order.Status}", 
                    $"{receipt.Order.Date}",
                    $"{receipt.Order.CustomerEmail}",
                    $"{totalPrice}"
                };

                csvData.Add(stringData);
            }

            using var sw = new StreamWriter($"sales_data.csv", false, Encoding.UTF8);
            // Write column headers
            sw.WriteLine("ID,OrderId,Status,Date,Customer Email,Amount");

            // Write data rows
            foreach (var line in csvData.Select(rowData => string.Join(",", rowData)))
            {
                sw.WriteLine(line);
            }

            _view.Info("Successfully export CSV file for Sales Data");
        } 
        catch (Exception ex) {
            Debug.Fail(ex.Message);
            _view.Error("Failed to display sales data");
        }
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
            _view.Info(message: $"Time: {receipt.Order.Date}");
        }
    }
}