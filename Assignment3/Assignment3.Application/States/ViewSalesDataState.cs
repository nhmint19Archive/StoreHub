using Assignment3.Application.Services;
using Assignment3.Domain.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Application.States
{
    internal class ViewSalesDataState : AppState
    {
        public void ShowDataOptions()
        {
            var options = new Dictionary<char, string>()
            {
                { 'E', "Exit to Main Menu" },
                { 'P', "Print Sales Data" }
            };

            var input = ConsoleHelper.AskUserOption(options);

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
            //TO DO: Change to Write Receipts after Transaction is implemented
            var product1 = new Product { Id = 1, Name = "Product 1", Description = "This is the first product", Price = 10.5m, InventoryCount = 10 };
            var product2 = new Product { Id = 2, Name = "Product 2", Description = "This is the second product", Price = 5.25m, InventoryCount = 20 };
            var productList = new List<Product>
            {
                product1,
                product2
        };

            string currentDir = System.IO.Directory.GetCurrentDirectory();
            var filePath = Path.GetFullPath(Path.Combine(currentDir, @"..\..\..\"));
            ConsoleHelper.PrintInfo($"{currentDir}");


            using var writer = new StreamWriter($"{filePath}\\fileTest.csv");
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(productList);
            }

        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
