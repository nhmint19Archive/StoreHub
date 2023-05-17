using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Domain.Data;
public class AppDbContext : DbContext
{
	public DbSet<Product> Products { get; set; }
	public DbSet<CustomerAccount> CustomerAccounts { get; set; }
	public DbSet<StaffAccount> StaffAccounts { get; set; }

	public AppDbContext()
	{
		_ = Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = "Data Source=AllYourHealthyDb.db";
		_ = optionsBuilder.UseSqlite(connectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// TODO: use the same table for these two
		_ = modelBuilder
			.Entity<StaffAccount>()
			.HasKey(x => x.Username);

		_ = modelBuilder
			.Entity<CustomerAccount>()
			.HasKey(x => x.Username);

		_ = modelBuilder
			.Entity<Product>()
			.HasKey(x => x.Id);
	}
}
