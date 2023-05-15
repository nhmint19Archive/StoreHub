using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Domain.Data;

/// <summary>
/// Represents a unit of work which coordinates multiple repositories.
/// </summary>
internal class UnitOfWork : IDisposable
{
	private readonly AppDbContext _dbContext;

	/// <summary>
	/// Product repository.
	/// </summary>
	public IQueryable<Product> Products => _dbContext.Products;

	public UnitOfWork()
	{
		_dbContext = new AppDbContext();
	}

	/// <summary>
	/// Commit to the changes made to repositories.
	/// </summary>
	/// <returns>Result object indicating the status of the data transaction.</returns>
	public DataResult CommitChanges()
	{
		try
		{
			_ = _dbContext.SaveChanges();
			return DataResult.Success;
		}
		catch (Exception exception)
		{
			return exception switch
			{
				DbUpdateException => DataResult.Conflict,
				ObjectDisposedException => DataResult.Disconnected,
				_ => DataResult.UnknownError,
			};
		}
	}

	/// <summary>
	/// Disposes the object once the data transactions are completed.
	/// </summary>
	public void Dispose()
	{
		_dbContext.Dispose();
	}
}
