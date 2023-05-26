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
        throw new NotImplementedException();
    }
}