using System.Diagnostics;
using System.Text;
using Assignment3.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Domain.Models;

public class SalesDataAnalyzer
{
    /// <summary>
    /// Write sales data to a file.
    /// </summary>
    public bool TryWriteSalesDataToFile(string fileName)
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
                var stringData = new [] { 
                    $"{receipt.Id}", 
                    $"{receipt.OrderId}", 
                    $"{receipt.Order.Status}", 
                    $"{receipt.Order.Date}",
                    $"{receipt.Order.CustomerEmail}",
                    $"{totalPrice}"
                };

                csvData.Add(stringData);
            }

            using var sw = new StreamWriter(fileName, false, Encoding.UTF8);
            // Write column headers
            sw.WriteLine("ID,OrderId,Status,Date,Customer Email,Amount");

            // Write data rows
            foreach (var line in csvData.Select(rowData => string.Join(",", rowData)))
            {
                sw.WriteLine(line);
            }

            return true;
        } 
        catch (Exception ex) {
            Debug.Fail(ex.Message);
            return false;
        }
    }
}