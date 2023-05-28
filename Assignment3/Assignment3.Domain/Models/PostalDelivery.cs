namespace Assignment3.Domain.Models;

public class PostalDelivery : IDeliveryMethod
{
	private readonly int _orderId;
	private readonly int _streetNumber;
	private readonly string _streetName;
	private readonly int _postcode;
	private readonly string? _apartmentNumber;

	public PostalDelivery(
		int orderId,
		int streetNumber,
		string streetName,
		int postcode,
		string? apartmentNumber)
	{
		_orderId = orderId;
		_streetNumber = streetNumber;
		_streetName = streetName;
		_postcode = postcode;
		_apartmentNumber = apartmentNumber;
	}

	public decimal DeliveryCost { get; } = 15;
	
	public void StartDelivery()
	{
		Console.WriteLine("Package has been accepted by Australian Post");
	}
}
