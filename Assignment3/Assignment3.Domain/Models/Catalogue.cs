using Assignment3.Domain.Data;

namespace Assignment3.Domain.Models;
public class Catalogue
{
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
