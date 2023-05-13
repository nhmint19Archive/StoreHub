using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Domain.Data;
internal class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\\mssqllocaldb;MultipleActiveResultSets=false;Database=Assignment3;Trusted_Connection=True;");
    }
}
