using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class Status
{
    public string StatusId { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<TourGroup> TourGroups { get; set; } = new List<TourGroup>();
}
