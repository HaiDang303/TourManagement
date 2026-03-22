using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class PaymentStatus
{
    public string StatusId { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
