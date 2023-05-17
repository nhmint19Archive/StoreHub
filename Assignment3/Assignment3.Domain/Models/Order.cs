using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Domain.Models;
public class Order
{
	public int Id { get; set; }
	public List<Dictionary<Product, uint>> Products { get; set; }
	public string Status { get; set; } = "In cart";
	public string ReceiptId { get; set; } = string.Empty;
	public string InvoiceId { get; set; } = string.Empty;

	public Order(int id, List<Dictionary<Product, uint>> products)
	{
		Id = id;
		Products = products;
	}

}
