namespace Assignment3.Domain.Models;

public abstract class DeliveryMethod
{
	protected readonly CustomerAccount _customer;
	protected readonly Order _order;

	public DeliveryMethod(Order order, CustomerAccount customer)
	{
		_order = order;
		_customer = customer;
	}

	public abstract decimal DeliveryCost { get; }

	public abstract void StartDelivery();
}
