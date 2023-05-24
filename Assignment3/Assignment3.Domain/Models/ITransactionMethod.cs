using System.Numerics;

namespace Assignment3.Domain.Models;
public interface ITransactionMethod
{
    Receipt Execute(Transaction transaction, Order order);
}
