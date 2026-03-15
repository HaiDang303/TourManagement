using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class BookingPassenger
{
    public string PassengerId { get; set; } = null!;

    public string BookingId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? GenderId { get; set; }

    public DateOnly? Dob { get; set; }

    public string CategoryId { get; set; } = null!;

    public string? PassportNo { get; set; }

    public string? Notes { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual PassengerCategory Category { get; set; } = null!;

    public virtual Gender? Gender { get; set; }
}
