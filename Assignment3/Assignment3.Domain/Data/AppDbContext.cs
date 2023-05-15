using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Domain.Data;
internal class AppDbContext : DbContext
{
	private const string SqlServerLocalDbName = @"(localdb)\mssqllocaldb";
	private const string DatabaseName = "AllYourHealthyFood";
	public DbSet<Product> Products { get; set; }
	public DbSet<CustomerAccount> CustomerAccounts { get; set; }
	public DbSet<StaffAccount> StaffAccounts { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = @$"Server={SqlServerLocalDbName};MultipleActiveResultSets=false;Database={DatabaseName};Trusted_Connection=True;";
		_ = optionsBuilder.UseSqlServer(connectionString);
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
