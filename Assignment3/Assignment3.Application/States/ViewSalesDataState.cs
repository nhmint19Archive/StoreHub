using Assignment3.Application.Models;
using Assignment3.Application.Services;
using Assignment3.Domain.Data;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace Assignment3.Application.States
{
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
            ShowDataOptions();
            ShowReceipts();
        }
        
        private void ShowDataOptions()
        {
            var options = new Dictionary<char, string>()
            {
                { 'E', "Exit to Main Menu" },
                { 'P', "Print Sales Data" }
            };

            var input = _inputHandler.AskUserOption(options);

            switch (input)
            {
                case 'P':
                    PrintSalesData();
                    break;
                case 'E':
                    OnStateChanged(this, nameof(MainMenuState));
                    break;
            }
        }

        private void PrintSalesData()
        {
            // we can change the path to print out later
            var currentDir = Directory.GetCurrentDirectory();
            //var filePath = Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\"));

            try
            {            
                using var context = new AppDbContext();
                var receipts = context.Receipts.AsNoTracking().AsEnumerable();
                using var writer = new StreamWriter($"fileTest.csv");
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(receipts);
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
}
