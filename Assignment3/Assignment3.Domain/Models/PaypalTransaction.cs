namespace Assignment3.Domain.Models;

public class PaypalTransaction : ITransactionMethod
{
    public Receipt Execute(Transaction transaction, Order order)
    {
        throw new NotImplementedException();
    }
}