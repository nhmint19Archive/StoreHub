namespace Assignment3.Domain.Models;

/// <summary>
/// Abstraction for different transaction strategies.
/// </summary>
public interface ITransactionMethod
{
    /// <summary>
    /// Executes the transaction for an order.
    /// </summary>
    /// <param name="transaction">Transaction.</param>
    /// <param name="order">Order being paid for.</param>
    /// <returns>Receipt.</returns>
    Receipt Execute(Transaction transaction, Order order);
}
