namespace StoreHub.Domain.Models;
public class Transaction
{
    public int Id { get; set; }
    public DateTime TransactionDateUtc { get; init; }
    public decimal Amount { get; init; }
}
