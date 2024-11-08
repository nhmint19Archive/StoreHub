﻿using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using StoreHub.Domain.Enums;

namespace StoreHub.Domain.Models;

public class UserAccount
{
	[Required]
	[StringLength(256, MinimumLength = 3, ErrorMessage = "Email length must be between 3 and 256 characters")]
	[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email is invalid")]
	public required string Email { get; init; }

	[Required]
	[StringLength(10, ErrorMessage = "Phone number must contains 10 digits")]
	[RegularExpression(@"^(\d{10})$", ErrorMessage = "Phone number must contains 10 digits")]
	public required string Phone { get; set; }

	[Required]
	public required Roles Role { get; init; }

	[Required]
	public DateTime RegistryDate { get; init; } = DateTime.UtcNow;

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; protected set; } = string.Empty;
	
	/// <summary>
	/// Sets the password to a hash of the raw password.
	/// </summary>
	/// <param name="password">Raw password string.</param>
	public void SetPassword(string password)
	{
		Password = Convert.ToHexString(HashPassword(password));
	}
	
	/// <summary>
	/// Authenticates the entered password.
	/// </summary>
	/// <param name="password">Raw password string.</param>
	/// <returns><c>True</c> if the hash of the password matches the stored hash; otherwise <c>False</c>.</returns>
	public bool Authenticate(string password)
	{
		var hashedString = Convert.ToHexString(HashPassword(password));
		return hashedString == Password;
	}

	private static byte[] HashPassword(string password)
	{
		var salt = new byte[1];
		var hashedPasswordBytes = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt, 
			100,
			HashAlgorithmName.SHA256,
			32);
		return hashedPasswordBytes;
	}
}
