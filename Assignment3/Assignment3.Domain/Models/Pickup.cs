namespace Assignment3.Domain.Models;

public class Pickup : DeliveryMethod
{
	public Pickup(Order order, CustomerAccount customer) : base(order, customer)
	{
	}

	public override decimal DeliveryCost => 0m;

	public override void ExecuteDelivery()
	{

	}
}
