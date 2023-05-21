using Assignment3.Domain.Data;

namespace Assignment3.Domain.Models;

public class Pickup : DeliveryMethod
{
	private readonly int _orderId;

	public Pickup(int orderId)
	{
		_orderId = orderId;
	}

	public override decimal DeliveryCost => 0m;

	public override void StartDelivery()
	{
		using var context = new AppDbContext();
		var order = context.Orders.Find(_orderId) ?? throw new InvalidOperationException();
		var email = new Email(order.CustomerEmail);
		email.Send("Order from All Your Healthy Food Store", "Your order is being processed, we will notify you where and when you can pick it up via email");
	}
}
