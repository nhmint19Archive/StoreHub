namespace Assignment3.Domain.Enums;

/// <summary>
/// Statuses an order can be in.
/// </summary>
public enum OrderStatus
{
	/// <summary>
	/// Unconfirmed order.
	/// </summary>
	Unconfirmed,

	/// <summary>
	/// Order has been paid for.
	/// </summary>
	Confirmed,

	/// <summary>
	/// Order has been delivered.
	/// </summary>
	Delivered
}
