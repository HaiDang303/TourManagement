using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class PassengerCategory
{
    public string CategoryId { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<BookingPassenger> BookingPassengers { get; set; } = new List<BookingPassenger>();
}
