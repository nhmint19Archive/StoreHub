using System.Collections.ObjectModel;
using Assignment3.Domain.Enums;

namespace Assignment3.Domain.Models;
public class Order
{
	private readonly string _customerEmail;

	private void UpdateStatus(object? sender, EventArgs arg)
	{
		Status = OrderStatus.Delivered;
	}

	public Order(string customerEmail)
	{
		_customerEmail = customerEmail;
	}

	public Invoice Prepare(DeliveryMethod deliveryMethod)
	{
		return new Invoice(
			new ReadOnlyCollection<OrderProduct>(Products),
			_customerEmail,
			deliveryMethod.DeliveryCost);
	}

	public int Id { get; set; }
	public OrderStatus Status { get; private set; } = OrderStatus.Unconfirmed;
	public IList<OrderProduct> Products { get; init; } = new List<OrderProduct>();
}
