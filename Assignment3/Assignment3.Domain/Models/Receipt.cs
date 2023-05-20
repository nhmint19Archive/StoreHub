using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Domain.Models;
public class Receipt
{
	public int Id { get; }
	public int OrderId { get; }
	public List<Dictionary<Product, uint>> Products { get; }
	 
	public decimal TotalPrice { get; }

	public int TransactionId { get;}

	public Receipt(List<Dictionary<Product, uint>> products, decimal totalPrice, int transactionId)
	{
		TotalPrice = totalPrice;
		TransactionId = transactionId;
		Products = products;
	}
}
