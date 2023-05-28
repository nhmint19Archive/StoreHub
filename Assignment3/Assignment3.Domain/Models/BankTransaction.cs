namespace Assignment3.Domain.Models;

public class BankTransaction : ITransactionMethod
{
    private readonly string _bsb;
    private readonly string _accountNo;

    public BankTransaction(string bsb, string accountNo)
    {
        _bsb = bsb;
        _accountNo = accountNo;
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