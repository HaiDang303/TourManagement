using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class Gender
{
    public string GenderId { get; set; } = null!;

    public string GenderName { get; set; } = null!;

    public virtual ICollection<BookingPassenger> BookingPassengers { get; set; } = new List<BookingPassenger>();
}
