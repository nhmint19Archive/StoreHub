using Assignment3.Domain.Data;
using Assignment3.Domain.Enums;

namespace Assignment3.Domain.Models;

public class Invoice
{
	private readonly IReadOnlyCollection<OrderProduct> _products;
	private readonly int _orderId;
	private readonly string _customerEmail;
	private readonly ITransactionMethod _transactionMethod;
	private readonly decimal _deliveryCost;

	internal Invoice(
		IReadOnlyCollection<OrderProduct> products,
		int orderId,
		string customerEmail,
		ITransactionMethod transactionMethod,
		decimal deliveryCost)
	{
		_products = products;
		_orderId = orderId;
		_customerEmail = customerEmail;
		_transactionMethod = transactionMethod;
		_deliveryCost = deliveryCost;
		TotalPrice = CalculateTotalPrice();
	}

	public int Id { get; set; }

	public decimal TotalPrice { get; }

	public void EmailToCustomer()
	{
		Console.WriteLine($"An invoice has been sent to '{_customerEmail}'");
		var email = new Email(_customerEmail);
		email.Send(
			"Your invoice from All Your Healthy Food Store",
			$"Order: {_orderId}\nTotal price: {TotalPrice}\nDelivery cost: {_deliveryCost}");
	}

	public bool MakePayment()
	{
        Console.WriteLine("Making payment");

        var transaction = new Transaction()
        {
	        TransactionDateUtc = DateTime.UtcNow,
	        Amount = TotalPrice,
        };

        try
        {
	        var receipt = _transactionMethod.Execute(transaction);
	        using var context = new AppDbContext();	        
	        var order = context.Orders.Find(_orderId) ?? throw new InvalidOperationException();
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

	private decimal CalculateTotalPrice()
	{
		return _products.Sum(x => x.PriceAtPurchase * x.ProductQuantity) + _deliveryCost;
	}
}
