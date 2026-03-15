using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class Destination
{
    public string DestinationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? Country { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
