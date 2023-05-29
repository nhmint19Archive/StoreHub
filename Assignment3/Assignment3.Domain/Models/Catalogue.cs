using Assignment3.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Assignment3.Domain.Models;

public class Catalogue
{
    private Func<decimal, bool>? _priceFilter = null;
    private Func<string, bool>? _nameFilter = null;

	public bool AreFiltersApplied => _priceFilter != null || _nameFilter != null;

    public void SetProductNameFilter(string productName)
    {
        _nameFilter = x => x.Contains(productName);
    }

    public void SetPriceFilters(decimal upperLimit = decimal.MaxValue, decimal lowerLimit = decimal.MinValue)
	{
		if (upperLimit < lowerLimit) 
		{ 
			throw new ArgumentException(
				 "Lower limit must be equal or lower than upper limit", 
				 nameof(upperLimit));
		}
		
		_priceFilter = x => x >= lowerLimit && x <= upperLimit;
    }

	public void ResetFilters()
	{
		_priceFilter = null;
		_nameFilter = null;
    }

	/// <summary>
	/// Gets a collection of available products.
	/// </summary>
	/// <param name="priceFilter">Optional price filter.</param>
	/// <param name="nameFilter">Optional product name filter.</param>
	/// <returns>Read-only collection of available products</returns>
	public IReadOnlyCollection<Product> GetProducts()
	{
		var priceFilter = _priceFilter ?? (_ => true);
        var nameFilter = _nameFilter ?? (_ => true);

        using var context = new AppDbContext();
		return context.Products
			.AsNoTracking()
			.Where(x => x.InventoryCount > 0)
			.AsEnumerable()
			.Where(x => nameFilter(x.Name) && priceFilter(x.Price))
			.ToList();
	}
}
