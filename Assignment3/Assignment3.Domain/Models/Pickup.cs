using Assignment3.Domain.Data;

namespace Assignment3.Domain.Models;

public class Pickup : IDeliveryMethod
{
	private readonly int _orderId;

	public Pickup(int orderId)
	{
		_orderId = orderId;
	}

	public decimal DeliveryCost => 0m;

	public void StartDelivery()
	{
		using var context = new AppDbContext();
		var order = context.Orders.Find(_orderId) ?? throw new InvalidOperationException();
		EmailSimulator.Send(
			order.CustomerEmail,
			"Order from All Your Healthy Food Store", 
			"Your order is being processed, we will notify you where and when you can pick it up via email");
	}
}
