using Assignment3.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Domain.Data;
internal class AppDbContext : DbContext
{
	public DbSet<Product> Products { get; set; }
	public DbSet<CustomerAccount> CustomerAccounts { get; set; }
	public DbSet<StaffAccount> StaffAccounts { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlServer(@"Server=(localdb)\\mssqllocaldb;MultipleActiveResultSets=false;Database=Assignment3;Trusted_Connection=True;");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// TODO: use the same table for these two
		modelBuilder
			.Entity<StaffAccount>()
			.HasKey(x => x.Username);

		modelBuilder
			.Entity<CustomerAccount>()
			.HasKey(x => x.Username);
	}
}
