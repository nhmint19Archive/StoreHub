namespace Assignment3.Domain.Models;

public class Receipt
{
	public int Id { get; init; }
	public int OrderId { get; init; }
    public int TransactionId { get; init; }
    // public IList<Product> Products { get; init; } = new List<Product>();
	public Transaction Transaction { get; init; } = null!;
}
