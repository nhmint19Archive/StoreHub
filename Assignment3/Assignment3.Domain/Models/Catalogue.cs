using Assignment3.Domain.Data;

namespace Assignment3.Domain.Models;
public class Catalogue
{
	public IReadOnlyCollection<Product> GetProducts(
		Func<Product, bool>? priceFilter = null,
		Func<Product, bool>? nameFilter = null)
	{
		using var context = new AppDbContext();
		var query = context.Products.Where(x => x.InventoryCount > 0);
		if (priceFilter is not null)
		{
			query = (IQueryable<Product>)query.Where(priceFilter);
		}

		if (nameFilter is not null)
		{
			query = (IQueryable<Product>)query.Where(nameFilter);
		}

		return query.ToList();
	}
}
