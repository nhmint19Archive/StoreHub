namespace Assignment3.Domain.Models;
public class OrderProduct
{
	public decimal PriceAtPurchase { get; set; }
	public int ProductQuantity { get; set; }
	public int ProductId { get; set; }
	public int OrderId { get; set; }
	public Order Order { get; init; } = null!;
	public Product Product { get; init; } = null!;
}
