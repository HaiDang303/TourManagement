using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public string BookingId { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string Method { get; set; } = null!;

    public string? TransactionRef { get; set; }

    public string StatusId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual PaymentStatus Status { get; set; } = null!;
}
