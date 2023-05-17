namespace Assignment3.Domain.Models;

public class CustomerAccount : UserAccount
{
	public CustomerAccount(string password) : base(password)
	{
	}

	public bool Active { get; init; }

	// TODO: create Order classes
	//public bool PlaceOrder(Order order)
	//{
	//}

	// TODO: create Catalogue class
	//public List<Product> SearchCatalogue(Catalogue catalogue, string keyword)
	//{
	//    return new List<Product>();
	//}
}
