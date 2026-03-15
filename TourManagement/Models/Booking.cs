using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public int UserId { get; set; }

    public string GroupId { get; set; } = null!;

    public DateTime BookingDate { get; set; }

    public int Adults { get; set; }

    public int Children { get; set; }

    public int Infants { get; set; }

    public decimal TotalPrice { get; set; }

    public string StatusId { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<BookingPassenger> BookingPassengers { get; set; } = new List<BookingPassenger>();

    public virtual TourGroup Group { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Status Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
