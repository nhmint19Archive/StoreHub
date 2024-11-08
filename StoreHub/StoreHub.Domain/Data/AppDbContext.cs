using System.Diagnostics;
using StoreHub.Domain.Enums;
using StoreHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StoreHub.Domain.Data;

/// <summary>
/// Implementation of a unit-of-work.
/// The DbSet{T} properties are implementations of repositories for entities of type T.
/// </summary>
public class AppDbContext : DbContext
{
	public DbSet<Product> Products { get; set; } = null!;
	public DbSet<UserAccount> UserAccounts { get; set; } = null!;
	public DbSet<Order> Orders { get; set; } = null!;
	public DbSet<OrderProduct> OrderProducts { get; set; } = null!;
	public DbSet<Receipt> Receipts { get; set; } = null!;
	public DbSet<Transaction> Transactions { get; set; } = null!;
	public DbSet<RefundRequest> RefundRequests { get; set; } = null!;
	
	/// <summary>
	/// Configures the class to use the specified datasource and database provider.
	/// </summary>
	/// <param name="optionsBuilder"></param>
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		const string connectionString = "Data Source=AllYourHealthyDb.db";
		_ = optionsBuilder.UseSqlite(connectionString);
	}

	/// <summary>
	/// An exception-safe wrapper for <see cref="DbContext.SaveChanges()"/>.
	/// Fails in Debug mode./>
	/// </summary>
	/// <returns><c>True</c> if the database transaction is successful; otherwise <c>False</c></returns>
	public bool TrySaveChanges()
	{
		try
		{
			SaveChanges();
			return true;
		}
		catch (Exception e)
		{
			Debug.Fail(e.Message, e.StackTrace);
			return false;
		}
	}

	/// <summary>
	/// Sets up relationships between entities.
	/// </summary>
	/// <param name="modelBuilder"></param>
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

		modelBuilder
			.Entity<RefundRequest>()
			.HasKey(x => x.Id);

		modelBuilder
			.Entity<RefundRequest>()
			.HasOne(x => x.Order);

    }
}
