using System.ComponentModel.DataAnnotations;

namespace Assignment3.Domain.Models;
public class Product
{
	[Required]
	public int Id { get; set; }
	
	[Required]
	[StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be at least 2 character-long")]
	public string Name { get; init; } = string.Empty;
	public string Description { get; init; } = string.Empty;
	public decimal Price { get; init; } = decimal.Zero;
	public uint InventoryCount { get; init; }
}
