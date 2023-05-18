using Assignment3.Domain.Enums;
using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Assignment3.Domain.Data;
public class AppDbContext : DbContext
{
	public DbSet<Product> Products { get; set; } = null!;
	public DbSet<UserAccount> UserAccounts { get; set; } = null!;

	public AppDbContext()
	{
		// comment out this line when performing migrations
		Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		const string connectionString = "Data Source=AllYourHealthyDb.db";
		_ = optionsBuilder.UseSqlite(connectionString);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		_ = modelBuilder
			.Entity<Product>()
			.HasKey(x => x.Id);

		var converter = new EnumToStringConverter<Roles>();
		modelBuilder
			.Entity<UserAccount>()
			.Property(x => x.Role)
			.HasConversion(converter);
			
		_ = modelBuilder
			.Entity<UserAccount>()
			.HasKey(x => x.Email);
	}
}
