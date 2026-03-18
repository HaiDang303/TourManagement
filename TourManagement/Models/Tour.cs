using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class Tour
{
    public string TourId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string DestinationId { get; set; } = null!;

    public int DurationDays { get; set; }

    public decimal BasePrice { get; set; }

    public string? Category { get; set; }

    public int? MaxParticipants { get; set; }

    public string? Description { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? GuideId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Destination Destination { get; set; } = null!;

    public virtual User? Guide { get; set; }

    public virtual ICollection<TourGroup> TourGroups { get; set; } = new List<TourGroup>();

    public virtual ICollection<TourPrice> TourPrices { get; set; } = new List<TourPrice>();
}
