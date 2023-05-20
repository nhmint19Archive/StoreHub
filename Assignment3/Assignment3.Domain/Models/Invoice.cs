namespace Assignment3.Domain.Models;

public class Invoice
{
	private readonly IReadOnlyCollection<OrderProduct> _products;
	private readonly string _customerEmail;
	private readonly decimal _deliveryCost;

	public Invoice(
		IReadOnlyList<OrderProduct> products,
		string customerEmail,
		decimal deliveryCost)
	{
		_products = products;
		_customerEmail = customerEmail;
		_deliveryCost = deliveryCost;
		TotalPrice = CalculateTotalPrice();
	}

	public int Id { get; set; }

	public decimal TotalPrice { get; }

	public void EmailInvoice()
	{
		Console.WriteLine($"An invoice has been sent to '{_customerEmail}'");
	}

	public void MakePayment(ITransactionStrategy transactionStrategy)
	{
        transactionStrategy.Execute();
        Console.WriteLine("Making payment");
	}

	private decimal CalculateTotalPrice()
	{
		return _products.Sum(x => x.PriceAtPurchase * x.ProductQuantity) + _deliveryCost;
	}
}
