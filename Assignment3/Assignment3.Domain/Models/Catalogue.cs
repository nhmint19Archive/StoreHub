using Assignment3.Domain.Data;
using System.Linq.Expressions;

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
		Expression<Func<Product, bool>>? priceFilter = null,
		Expression<Func<Product, bool>>? nameFilter = null)
	{
		using var context = new AppDbContext();
		var query = context.Products.Where(x => x.InventoryCount > 0);
		
		if (priceFilter != null)
		{
			query = query.Where(priceFilter);
		}

		if (nameFilter != null)
		{
			query = query.Where(nameFilter);
		}

		return query.ToList();
	}
}
