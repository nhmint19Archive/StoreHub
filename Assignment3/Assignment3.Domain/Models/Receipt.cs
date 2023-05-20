namespace Assignment3.Domain.Models;

public class Receipt
{
	public int Id { get; init; }

	public int OrderId { get; init; }

	public ICollection<Product> Products { get; init; } = new List<Product>();

	public Transaction Transaction { get; init; } = null!;

	public int TransactionId { get; init; }
}
