using System.ComponentModel.DataAnnotations;
using Assignment3.Domain.Enums;

namespace Assignment3.Domain.Models;

public class UserAccount
{
	[Required]
	[StringLength(256, MinimumLength = 3, ErrorMessage = "Email length must be between 3 and 256 characters")]
	[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
	public required string Email { get; init; }

	[Required]
	[StringLength(10, ErrorMessage = "Phone number must contains 10 digits")]
	[RegularExpression(@"^(\d{10})$", ErrorMessage = "Phone number must contains 10 digits")]
	public required string Phone { get; init; }

	[Required]
	public required Roles Role { get; init; }

	[Required]
	public DateTime RegistryDate { get; } = DateTime.UtcNow;

	[Required]
	[DataType(DataType.Password)]
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
