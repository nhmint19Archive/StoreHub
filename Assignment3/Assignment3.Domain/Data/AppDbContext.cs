using System.Diagnostics.CodeAnalysis;
using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Domain.Data;
public class AppDbContext : DbContext
{
	public DbSet<Product> Products { get; set; } = null!;
	public DbSet<CustomerAccount> CustomerAccounts { get; set; } = null!;
	public DbSet<StaffAccount> StaffAccounts { get; set; } = null!;
	public DbSet<Invoice> Invoices { get; set; } = null!;
	public DbSet<Receipt> Receipts { get; set; } = null!;
	public DbSet<Transaction> Transactions { get; set; } = null!;
	public DbSet<Order> Orders { get; set; } = null!;

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
		_ = modelBuilder
			.Entity<Invoice>()
			.HasKey(x => x.Id);
		_ = modelBuilder
			.Entity<Receipt>()
			.HasKey(x => x.Id);	
		_ = modelBuilder
			.Entity<Transaction>()	
			.HasKey(x => x.Id);
	}
}
