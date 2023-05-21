namespace Assignment3.Domain.Models;

public abstract class DeliveryMethod
{
	public abstract decimal DeliveryCost { get; }

	public abstract void StartDelivery();
}
