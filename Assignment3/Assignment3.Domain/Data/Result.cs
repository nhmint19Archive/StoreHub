namespace Assignment3.Domain.Data;

/// <summary>
/// Represents the result of a data transaction.
/// </summary>
public enum DataResult
{
	/// <summary>
	/// Successful data transaction.
	/// </summary>
	Success,

	/// <summary>
	/// Transaction failed due to conflict with existing data.
	/// </summary>
	Conflict,

	/// <summary>
	/// Transaction failed because the related entity was not found.
	/// </summary>
	NotFound,

	/// <summary>
	/// Transaction failed because the connection to the data store failed.
	/// </summary>
	Disconnected,

	/// <summary>
	/// Transaction failed due to an unknown error.
	/// </summary>
	UnknownError
}
