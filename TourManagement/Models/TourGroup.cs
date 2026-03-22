using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class TourGroup
{
    public string GroupId { get; set; } = null!;

    public string TourId { get; set; } = null!;

    public DateOnly DepartDate { get; set; }

    public DateOnly ReturnDate { get; set; }

    public int MaxCapacity { get; set; }

    public int CurrentBookings { get; set; }

    public string StatusId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual TourStatus Status { get; set; } = null!;

    public virtual Tour Tour { get; set; } = null!;
}
