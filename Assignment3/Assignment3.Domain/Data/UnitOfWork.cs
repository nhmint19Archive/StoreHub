using Assignment3.Domain.Models;

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
    /// Save the changes made to repositories.
    /// </summary>
    /// <returns>Result object indicating the status of the data transaction.</returns>
    public DataResult Save()
    {
        _dbContext.SaveChanges();
        return DataResult.Success;
    }

    /// <summary>
    /// Disposes the object once the data transactions are completed.
    /// </summary>
    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
