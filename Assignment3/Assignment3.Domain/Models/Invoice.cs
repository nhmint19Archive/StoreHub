using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Domain.Models;
public class Invoice
{
	public int Id { get; set; }
	public List<Dictionary<Product, uint>> Products;
	public decimal TotalPrice { get; }

	public Invoice(List<Dictionary<Product, uint>> products, decimal totalPrice)
	{
		Products = products;
		TotalPrice = totalPrice;
	}

}
