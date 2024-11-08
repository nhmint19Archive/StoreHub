using StoreHub.Domain.Enums;

namespace StoreHub.Domain.Models;

public class RefundRequest
{
    public int Id { get; set; }
    public int OrderId { get; init; }
    public Order Order { get; init; } = null!;
    public string Description { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string StaffComment { get; set; } = string.Empty;
    public RefundRequestStatus RequestStatus { get; set; } = RefundRequestStatus.Processing;
}