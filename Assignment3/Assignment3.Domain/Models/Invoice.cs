using System.Diagnostics.Contracts;
using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Domain.Models;

internal class Invoice
{
	private readonly IReadOnlyCollection<OrderProduct> _orderProducts;
	private readonly int _orderId;
	private readonly string _customerEmail;
	private readonly ITransactionMethod _transactionMethod;
	private readonly decimal _deliveryCost;
	private readonly decimal _totalPrice;

	internal Invoice(
		IReadOnlyCollection<OrderProduct> orderProducts,
		int orderId,
		string customerEmail,
		ITransactionMethod transactionMethod,
		decimal deliveryCost)
	{
		_orderProducts = orderProducts;
		_orderId = orderId;
		_customerEmail = customerEmail;
		_transactionMethod = transactionMethod;
		_deliveryCost = deliveryCost;

		PopulateProductPrice();
		_totalPrice = _orderProducts.Sum(x => x.PriceAtPurchase * x.ProductQuantity) + _deliveryCost;
	}
	
	/// <summary>
	/// Email the invoice to the customer.
	/// </summary>
	internal void EmailToCustomer()
	{
		EmailSimulator.Send(
			_customerEmail,
			"Your invoice from All Your Healthy Food Store",
			$"Order: {_orderId}\nTotal price: {_totalPrice}\nDelivery cost: {_deliveryCost}");
	}
	
	/// <summary>
	/// Make the payment.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	internal bool MakePayment()
	{
        Console.WriteLine("Making payment");
        try
        {
	        using var context = new AppDbContext();	        
	        var order = context.Orders
		        .Include(x => x.Products)
		        .ThenInclude(x => x.Product)
		        .FirstOrDefault(x => x.Id == _orderId) ?? throw new InvalidOperationException("Order not found");
	        
	        var transaction = new Transaction()
	        {
		        TransactionDateUtc = DateTime.UtcNow,
		        Amount = _totalPrice,
	        };

	        foreach (var purchasedOrderProducts in order.Products)
	        {
		        purchasedOrderProducts.Product.InventoryCount -= purchasedOrderProducts.ProductQuantity;
		        if (purchasedOrderProducts.Product.InventoryCount < 0)
		        {
			        throw new InvalidOperationException("Cannot purchase more products than the available stock");
		        }
	        }
	        
	        var receipt = _transactionMethod.Execute(transaction, order);

	        order.Status = OrderStatus.Confirmed;

	        context.Update(order);
	        context.Transactions.Add(transaction);
	        context.Receipts.Add(receipt);

	        context.SaveChanges();

	        return true;
        }
        catch
        {
	        return false;
        }
	}

	private void PopulateProductPrice()
	{
		using var context = new AppDbContext();
		var productsToPurchase = context.Products
			.Where(x => _orderProducts.Select(y => y.ProductId).Contains(x.Id))
			.ToDictionary(x => x.Id, x => x.Price);
		
		foreach (var orderProduct in _orderProducts)
		{
			orderProduct.PriceAtPurchase = productsToPurchase[orderProduct.ProductId];
		}
	}
}
