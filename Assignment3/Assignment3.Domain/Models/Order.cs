using System.Collections.ObjectModel;
using Assignment3.Domain.Enums;

namespace Assignment3.Domain.Models;
public class Order
{
	private DeliveryMethod? _deliveryMethod;
	public Order(string customerEmail)
	{
		CustomerEmail = customerEmail;
	}

	public Invoice Prepare(
		DeliveryMethod deliveryMethod,
		ITransactionStrategy transactionStrategy)
	{
		return new Invoice(
			new ReadOnlyCollection<OrderProduct>(Products),
			Id,
			CustomerEmail,
			transactionStrategy,
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
