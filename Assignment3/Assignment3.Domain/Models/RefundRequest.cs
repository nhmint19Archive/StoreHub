using Assignment3.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Domain.Models
{
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
}
