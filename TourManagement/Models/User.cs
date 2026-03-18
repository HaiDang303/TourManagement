using System;
using System.Collections.Generic;

namespace TourManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Address { get; set; }

    public string? Gender { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Tour> TourCreatedByNavigations { get; set; } = new List<Tour>();

    public virtual ICollection<Tour> TourGuides { get; set; } = new List<Tour>();
}
