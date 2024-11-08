namespace StoreHub.Domain.Models;

/// <summary>
/// Abstraction for different delivery options.
/// </summary>
public interface IDeliveryMethod
{ 
	/// <summary>
	/// Delivery cost.
	/// </summary>
	decimal DeliveryCost { get; }
	
	/// <summary>
	/// Start the delivery process.
	/// </summary>
	void StartDelivery();
}
