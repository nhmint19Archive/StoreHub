namespace Assignment3.Domain.Models;

public class CashTransaction : ITransactionMethod
{
    public Receipt Execute(Transaction transaction, Order order)
    {
        return new Receipt
        {
            Transaction = transaction,
            Order = order,
            TransactionId = transaction.Id,
            OrderId = order.Id,
        };
    }
}