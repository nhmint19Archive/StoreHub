using Assignment3.Domain.Data;

namespace Assignment3.Domain.Models;

public class Pickup : IDeliveryMethod
{
	private readonly int _orderId;

	public Pickup(int orderId)
	{
		_orderId = orderId;
	}
	
	/// <summary>
	/// Pickup is free.
	/// </summary>
	public decimal DeliveryCost => 0m;

	/// <summary>
	/// Simulates the pick up process.
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	public void StartDelivery()
	{
		using var context = new AppDbContext();
		var order = context.Orders.Find(_orderId) ?? throw new InvalidOperationException();
		EmailSimulator.Send(
			order.CustomerEmail,
			"Order from All Your Healthy Food Store", 
			"Your order is being processed, we will notify you where and when you can pick it up via email");
		EmailSimulator.Send(
			order.CustomerEmail,
			"Order from All Your Healthy Food Store", 
			"Your order is available for pickup from 9AM to 5PM at our Glenferrie store.");
	}
}
