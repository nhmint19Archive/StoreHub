using Assignment3.Domain.Data;

namespace Assignment3.Domain.Models;
public class Catalogue
{
	/// <summary>
	/// Gets a collection of available products.
	/// </summary>
	/// <param name="priceFilter">Optional price filter.</param>
	/// <param name="nameFilter">Optional product name filter.</param>
	/// <returns>Read-only collection of available products</returns>
	public IReadOnlyCollection<Product> GetProducts(
		Func<Product, bool>? priceFilter = null,
		Func<Product, bool>? nameFilter = null)
	{
		using var context = new AppDbContext();
		var query = context.Products
			.Where(x => x.InventoryCount > 0)
			.Where(priceFilter != null ? priceFilter : x => true)
			.Where(nameFilter != null ? nameFilter : x => true);

		return query.ToList();
	}
}
