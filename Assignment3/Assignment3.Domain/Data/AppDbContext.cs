using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Assignment3.Domain.Data;
public class AppDbContext : DbContext
{
	public DbSet<Product> Products { get; set; } = null!;
	public DbSet<UserAccount> UserAccounts { get; set; } = null!;
	public DbSet<Order> Orders { get; set; } = null!;
	public DbSet<OrderProduct> OrderProducts { get; set; } = null!;
	public DbSet<Receipt> Receipts { get; set; } = null!;
	public DbSet<Transaction> Transactions { get; set; } = null!;

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		const string connectionString = "Data Source=AllYourHealthyDb.db";
		_ = optionsBuilder.UseSqlite(connectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder
			.Entity<Product>()
			.HasKey(x => x.Id);

		modelBuilder
			.Entity<UserAccount>()
			.Property(x => x.Role)
			.HasConversion(new EnumToStringConverter<Roles>());

		modelBuilder
			.Entity<UserAccount>()
			.HasKey(x => x.Email);

		modelBuilder
			.Entity<Order>()
			.HasKey(x => x.Id);

		modelBuilder
			.Entity<Order>()
			.HasOne<UserAccount>()
			.WithMany()
			.HasForeignKey(x => x.CustomerEmail);

		modelBuilder
			.Entity<Receipt>()
			.HasKey(x => x.Id);

		modelBuilder
			.Entity<Receipt>()
			.HasOne(x => x.Transaction);

		modelBuilder
			.Entity<Transaction>()
			.HasKey(x => x.Id);

		modelBuilder
			.Entity<OrderProduct>()
			.HasKey(x => new { x.OrderId, x.ProductId });
    }
}
