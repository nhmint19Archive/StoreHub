namespace Assignment3.Domain.Models;
public class OrderProduct
{
	public decimal PriceAtPurchase { get; init; }
	public uint ProductQuantity { get; init; }
	public int ProductId { get; set; }
	public int OrderId { get; set; }
	public Order Order { get; init; }
	public Product Product { get; init; }
}
