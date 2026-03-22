using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class BookingStatus
{
    public string StatusId { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
