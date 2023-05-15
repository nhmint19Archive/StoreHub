using Assignment3.Domain.Data;

namespace Assignment3.Domain.Models;
public class Catalogue
{
	public IReadOnlyCollection<Product> GetProducts(
		Func<decimal, bool>? priceFilter = null,
		Func<string, bool>? nameFilter = null)
	{
		using var context = new AppDbContext();
		var query = context.Products.Where(x => x.InventoryCount > 0);
		if (priceFilter is not null)
		{
			query = query.Where(x => priceFilter(x.Price));
		}

		if (nameFilter is not null)
		{
			query = query.Where(x => nameFilter(x.Name));
		}

		return query.ToList();
	}
}
