namespace Assignment3.Domain.Models;

public class CreditCardTransaction : ITransactionMethod
{
    private readonly string _accountNo;
    private readonly string _cvc;
    private readonly DateOnly _expiryDate;

    public CreditCardTransaction(string accountNo, string cvc, DateOnly expiryDate)
    {
        _accountNo = accountNo;
        _cvc = cvc;
        _expiryDate = expiryDate;
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