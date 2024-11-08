using StoreHub.Domain.Data;

namespace StoreHub.Domain.Models;

public class PostalDelivery : IDeliveryMethod
{
	private readonly int _orderId;
	private readonly int _streetNumber;
	private readonly string _streetName;
	private readonly int _postcode;
	private readonly string _apartmentNumber;
	private readonly string _customerEmail;

	public PostalDelivery(
		int orderId,
		int streetNumber,
		string streetName,
		int postcode,
		string apartmentNumber,
		string customerEmail)
	{
		_orderId = orderId;
		_streetNumber = streetNumber;
		_streetName = streetName;
		_postcode = postcode;
		_apartmentNumber = apartmentNumber ;
		_customerEmail = customerEmail;
	}
	
	/// <summary>
	/// Arbitrary delivery cost.
	/// </summary>
	public decimal DeliveryCost { get; } = 15;
	
	/// <summary>
	/// Simulates a package delivery from Australian Post
	/// </summary>
	public void StartDelivery()
	{		
		EmailSimulator.Send(
			_customerEmail,
			"Order from All Your Healthy Food Store", 
			$"Package for order [{_orderId}] has been accepted by Australian Post.\nDelivery addressed to {_apartmentNumber} {_streetNumber} {_streetName} {_postcode}");

        using var context = new AppDbContext();
        var order = context.Orders.Find(_orderId) ?? throw new InvalidOperationException();

		// sometime later
        order.Status = Enums.OrderStatus.Delivered;
        context.Orders.Update(order);
        context.SaveChanges();
    }
}
