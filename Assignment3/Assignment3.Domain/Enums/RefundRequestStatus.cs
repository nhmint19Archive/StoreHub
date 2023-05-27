namespace Assignment3.Domain.Enums;

/// <summary>
/// Statuses an order can be in.
/// </summary>
public enum RefundRequestStatus
{
    /// <summary>
    /// Request has been sent
    /// </summary>
    Processing,

    /// <summary>
    /// Request has been reviewd by staff
    /// </summary>
    Reviewed,

    /// <summary>
    /// Request has been confirmed for refund
    /// </summary>
    Accepted
}
