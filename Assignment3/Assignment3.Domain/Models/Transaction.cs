namespace Assignment3.Domain.Models;
public abstract class Transaction
{
	public int Id { get; set; }

	public DateTime TransactionDateUtc { get; init; }

	public abstract Receipt Execute();
}
