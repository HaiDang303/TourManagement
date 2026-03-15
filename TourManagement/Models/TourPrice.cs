using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class TourPrice
{
    public string PriceId { get; set; } = null!;

    public string TourId { get; set; } = null!;

    public decimal Price { get; set; }

    public DateOnly ValidFrom { get; set; }

    public DateOnly ValidTo { get; set; }

    public string? Notes { get; set; }

    public virtual Tour Tour { get; set; } = null!;
}
