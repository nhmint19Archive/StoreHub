namespace Assignment3.Domain.Models;

public class PaypalTransaction : ITransactionMethod
{
    public Receipt Execute(Transaction transaction)
    {
        throw new NotImplementedException();
    }
}