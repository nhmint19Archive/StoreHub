﻿using System.Collections.ObjectModel;
using StoreHub.Domain.Enums;

namespace StoreHub.Domain.Models;
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
		if (_invoice == null)
		{
			throw new InvalidOperationException("Cannot confirm an order that is not finalized");
		}
		
		if (!_invoice.TryMakePayment(out var receipt))
		{
			return false;
		}
			
		receipt.EmailToCustomer();
		Status = OrderStatus.Confirmed;
		StartDelivery();
		return true;
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
