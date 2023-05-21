namespace Assignment3.Domain.Models;

public interface IDeliveryMethod
{ 
	decimal DeliveryCost { get; }
	void StartDelivery();
}
