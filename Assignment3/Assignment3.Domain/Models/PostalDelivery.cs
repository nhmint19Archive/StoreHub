namespace Assignment3.Domain.Models;

public class PostalDelivery : IDeliveryMethod
{
	private readonly int _orderId;
	private readonly int _streetNumber;
	private readonly string _streetName;
	private readonly int _postcode;
	private readonly string _apartmentNumber;

	public PostalDelivery(
		int orderId,
		int streetNumber,
		string streetName,
		int postcode,
		string apartmentNumber = "")
	{
		_orderId = orderId;
		_streetNumber = streetNumber;
		_streetName = streetName;
		_postcode = postcode;
		_apartmentNumber = apartmentNumber ;
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
		Console.WriteLine($"Package for order [{_orderId}] has been accepted by Australian Post");
		Console.WriteLine($"Delivery addressed to {_apartmentNumber} {_streetNumber} {_streetName} {_postcode}");
	}
}
