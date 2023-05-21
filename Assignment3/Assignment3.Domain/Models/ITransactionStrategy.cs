namespace Assignment3.Domain.Models;
public interface ITransactionStrategy
{
    Receipt Execute(Transaction transaction);
}
