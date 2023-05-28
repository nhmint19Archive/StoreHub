using System.Collections.ObjectModel;
using Assignment3.Domain.Enums;

namespace Assignment3.Domain.Models;
public class Order
{
	private IDeliveryMethod? _deliveryMethod;
	public Order(string customerEmail)
	{
		CustomerEmail = customerEmail;
		Date = DateTime.UtcNow;
	}

	public Invoice Prepare(
		IDeliveryMethod deliveryMethod,
		ITransactionMethod transactionMethod)
	{
		_deliveryMethod = deliveryMethod;
		return new Invoice(
			new ReadOnlyCollection<OrderProduct>(Products),
			Id,
			CustomerEmail,
			transactionMethod,
			deliveryMethod.DeliveryCost);
	}

	public void StartDelivery()
	{
		if (_deliveryMethod == null)
		{
			throw new InvalidOperationException();
		}
		
		_deliveryMethod.StartDelivery();
	}

	public int Id { get; set; }
	public DateTime Date { get; init; }
    public string CustomerEmail { get; init; }
    public OrderStatus Status { get; set; } = OrderStatus.Unconfirmed;
	public IList<OrderProduct> Products { get; init; } = new List<OrderProduct>();
}
