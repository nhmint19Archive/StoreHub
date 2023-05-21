namespace Assignment3.Domain.Models;

public class PostalDelivery : DeliveryMethod
{
	public PostalDelivery(Order order, CustomerAccount customer) : base(order, customer)
	{
	}

	public override decimal DeliveryCost => throw new NotImplementedException();

	public override void StartDelivery()
	{

	}
}
