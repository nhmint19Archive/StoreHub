namespace Assignment3.Domain.Models;

public class UserAccount
{
	public required string Username { get; init; }

	public required string Email { get; init; }

	public required string Phone { get; init; }

	public required DateTime? RegistryDate { get; init; }

	protected string Password { get; set; } = string.Empty;

	public bool Authenticate(string username, string password)
	{
		// TODO: encrypt password
		return Username == username && Password == password;
	}
}
