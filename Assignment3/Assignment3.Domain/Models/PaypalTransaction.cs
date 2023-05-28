namespace Assignment3.Domain.Models;

public class PaypalTransaction : ITransactionMethod
{
    private readonly string _username;
    public PaypalTransaction(string username)
    {
        _username = username;
    }
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