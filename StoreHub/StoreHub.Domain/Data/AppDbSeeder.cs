using StoreHub.Domain.Enums;
using StoreHub.Domain.Models;

namespace StoreHub.Domain.Data;

public static class AppDbSeeder
{
	/// <summary>
	/// Seed products and user accounts into the database in Debug mode.
	/// In Release mode, only an Admin account is seeded.
	/// </summary>
	public static void SeedData()
	{
		using var context = new AppDbContext();

#if DEBUG
		context.Products.RemoveRange(context.Products);
		context.UserAccounts.RemoveRange(context.UserAccounts);

		// seed products
		context.Products.AddRange(new List<Product>()
		{
			new Product()
			{
				Name = "Cabbage",
				Description = "Fresh cabbage from local farm",
				Price = 3,
				InventoryCount = 12
			},
			new Product()
			{
				Name = "Ham",
				Description = "Fresh ham from local farm",
				Price = 10,
				InventoryCount = 34
			},
			new Product()
			{
				Name = "Beef",
				Description = "Fresh beef from local farm",
				Price = 15,
				InventoryCount = 29
			},
			new Product()
			{
				Name = "Corn",
				Description = "Fresh corn from local farm",
				Price = 6,
				InventoryCount = 5
			},
			new Product()
			{
				Name = "Water",
				Description = "Water from nearby fountain",
				Price = 2,
				InventoryCount = 17
			},
			new Product()
			{
				Name = "Sparkling Water",
				Description = "Still water but sparkle",
				Price = 3,
				InventoryCount = 16
			},
			new Product()
			{
				Name = "Carrot",
				Description = "Fresh carrot from local farm",
				Price = 4,
				InventoryCount = 1
			},
			new Product()
			{
				Name = "Bread",
				Description = "Morning baked bread",
				Price = 8,
				InventoryCount = 0
			},
			new Product()
			{
				Name = "Lamb",
				Description = "Fresh lamb from local farm",
				Price = 20,
				InventoryCount = 25
			},
			new Product()
			{
				Name = "Tomato",
				Description = "Fresh tomato from local garden",
				Price = 7,
				InventoryCount = 50
			}
		});
		
		// seed user accounts
		var user1 = new UserAccount
		{
			Email = "minity@mail.com",
			Phone = "0123456789",
			Role = Roles.Customer,
		};

		user1.SetPassword("minity");
		var user2 = new UserAccount
		{
			Email = "staff2@mail.com",
			Phone = "0123456780",
			Role = Roles.Staff,
		};

		user2.SetPassword("admin");
		var user3 = new UserAccount
		{
			Email = "customer@mail.com",
			Phone = "0123456788",
			Role = Roles.Customer,
		};

		user3.SetPassword("customer");
		var user4 = new UserAccount
		{
			Email = "staff@mail.com",
			Phone = "0123456787",
			Role = Roles.Staff,
		};

		user4.SetPassword("staff");
		var user5 = new UserAccount
		{
			Email = "customer2@mail.com",
			Phone = "0123456786",
			Role = Roles.Customer,
		};

		user5.SetPassword("customer");
		context.UserAccounts.AddRange(new List<UserAccount>() { user1, user2, user3, user4, user5 });
#endif
		
		// seed admin account if not exists
		if (!context.UserAccounts.Any(u => u.Role == Roles.Admin))
		{
			var admin = new UserAccount
			{
				Email = "admin@mail.com",
				Phone = "0000000000",
				Role = Roles.Admin,
			};
			admin.SetPassword("admin");
			context.UserAccounts.Add(admin);
			context.SaveChanges();
		}
	}
}