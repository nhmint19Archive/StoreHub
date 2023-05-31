namespace Assignment3.Domain.Models;

public class Receipt
{
	public int Id { get; init; }
	public int OrderId { get; init; }
	public Order Order { get; init; } = null!;
    public int TransactionId { get; init; }
	public Transaction Transaction { get; init; } = null!;
	
	public void EmailToCustomer()
	{
		EmailSimulator.Send(
			Order.CustomerEmail,
			"Your receipt from All Your Healthy Food Store",
			$"Order: [{Order.Id}]\nReceipt: [{Id}]\nConfirmed Date: {DateTime.UtcNow.ToLocalTime()}");
	}

}
