namespace Assignment3.Domain.Models;

public class PaypalTransaction : ITransactionMethod
{
    private readonly string _username;

    public PaypalTransaction(string username)
    {
        _username = username;
    }
    public Receipt Execute(Transaction transaction, int orderId)
    {
        return new Receipt
        {
            OrderId = orderId,
            Transaction = transaction,
            TransactionId = transaction.Id
        };
    }
}