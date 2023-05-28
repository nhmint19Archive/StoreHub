namespace Assignment3.Domain.Models;

public class PaypalTransaction : ITransactionMethod
{
    private readonly string _username;
    public PaypalTransaction(string username)
    {
        _username = username;
    }
    
    /// <summary>
    /// Simulates a paypal transaction.
    /// </summary>
    /// <param name="transaction">Transaction.</param>
    /// <param name="order">Order being paid for.</param>
    /// <returns>Receipt</returns>
    public Receipt Execute(Transaction transaction, Order order)
    {
        Console.WriteLine("Redirecting you to Paypal.com");
        Console.WriteLine("...");
        Console.WriteLine("Paypal payment successful");
        return new Receipt
        {
            Transaction = transaction,
            Order = order,
            TransactionId = transaction.Id,
            OrderId = order.Id,
        };
    }
}