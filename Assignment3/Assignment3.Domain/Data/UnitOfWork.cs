using Assignment3.Domain.Models;

namespace Assignment3.Domain.Data;
internal class UnitOfWork : IDisposable
{
    private readonly AppDbContext _dbContext;
    public IQueryable<Product> Products => _dbContext.Products;

    public UnitOfWork()
    {
        _dbContext = new AppDbContext();
    }

    public DataResult Save()
    {
        _dbContext.SaveChanges();
        return DataResult.Success;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
