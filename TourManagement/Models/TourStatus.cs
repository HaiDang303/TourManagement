using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class TourStatus
{
    public string StatusId { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<TourGroup> TourGroups { get; set; } = new List<TourGroup>();
}
