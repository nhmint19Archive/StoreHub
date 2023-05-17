using Assignment3.Domain.Enums;

namespace Assignment3.Domain.Models;

public class UserAccount
{
	public required string Email { get; init; }
	public required string Phone { get; init; }
	public required Roles Role { get; init; }
	public DateTime? RegistryDate { get; } = DateTime.UtcNow;
	public string Password { get; protected set; } = string.Empty;
	public UserAccount(string password)
	{
		Password = password;
	}

	public bool Authenticate(string password)
	{
		// TODO: encrypt password
		return Password == password;
	}
}
