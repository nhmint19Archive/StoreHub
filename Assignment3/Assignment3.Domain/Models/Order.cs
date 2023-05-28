using System.Collections.ObjectModel;
using Assignment3.Domain.Enums;

namespace Assignment3.Domain.Models;
public class Order
{
	private IDeliveryMethod? _deliveryMethod;
	private Invoice? _invoice = null;
	public Order(string customerEmail)
	{
		CustomerEmail = customerEmail;
		Date = DateTime.UtcNow;
	}
	
	/// <summary>
	/// Finalize the order delivery and transaction methods.
	/// </summary>
	/// <param name="deliveryMethod">The chosen delivery method.</param>
	/// <param name="transactionMethod">The chosen transaction method.</param>
	public void Finalize(
		IDeliveryMethod deliveryMethod,
		ITransactionMethod transactionMethod)
	{
		_deliveryMethod = deliveryMethod;
		_invoice = new Invoice(
			new ReadOnlyCollection<OrderProduct>(Products),
			Id,
			CustomerEmail,
			transactionMethod,
			deliveryMethod.DeliveryCost);
		_invoice.EmailToCustomer();
	}
	
	/// <summary>
	/// Trigger the payment and if successful, starts the delivery.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Order has not been finalized.</exception>
	public bool Confirm()
	{
		var success = _invoice?.MakePayment() 
			?? throw new InvalidOperationException("Cannot confirm an order that is not finalized");
		if (!success)
		{
			return false;
		}
		
		StartDelivery();
		SendReceiptToCustomer();
		return true;
	}

	private void SendReceiptToCustomer()
	{
		EmailSimulator.Send(
			CustomerEmail,
			"Your receipt from All Your Healthy Food Store",
			$"Order: {Id}\nConfirmed Date: {DateTime.UtcNow.ToLocalTime()}");
	}

	private void StartDelivery()
	{
		if (_deliveryMethod == null)
		{
			throw new InvalidOperationException();
		}
		
		_deliveryMethod?.StartDelivery();
	}

	public int Id { get; set; }
	public DateTime Date { get; init; }
    public string CustomerEmail { get; init; }
    public OrderStatus Status { get; set; } = OrderStatus.Unconfirmed;
	public IList<OrderProduct> Products { get; set; } = new List<OrderProduct>();
}
