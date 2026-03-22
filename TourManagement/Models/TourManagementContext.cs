using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TourManagement.Models;

public partial class TourManagementContext : DbContext
{
    public TourManagementContext()
    {
    }

    public TourManagementContext(DbContextOptions<TourManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingPassenger> BookingPassengers { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<Destination> Destinations { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<PassengerCategory> PassengerCategories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<TourGroup> TourGroups { get; set; }

    public virtual DbSet<TourPrice> TourPrices { get; set; }

    public virtual DbSet<TourStatus> TourStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=TourManagement;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__5DE3A5B17A58FC83");

            entity.Property(e => e.BookingId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("booking_id");
            entity.Property(e => e.Adults)
                .HasDefaultValue(1)
                .HasColumnName("adults");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.Children).HasColumnName("children");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("group_id");
            entity.Property(e => e.Infants).HasColumnName("infants");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.StatusId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("PENDING")
                .HasColumnName("status_id");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Group).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_TourGroups");

            entity.HasOne(d => d.Status).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_BookingStatuses");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");
        });

        modelBuilder.Entity<BookingPassenger>(entity =>
        {
            entity.HasKey(e => e.PassengerId).HasName("PK__BookingP__03764586221805B5");

            entity.Property(e => e.PassengerId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("passenger_id");
            entity.Property(e => e.BookingId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("booking_id");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("category_id");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.GenderId)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("gender_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(200)
                .HasColumnName("notes");
            entity.Property(e => e.PassportNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("passport_no");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingPassengers)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingPassengers_Bookings");

            entity.HasOne(d => d.Category).WithMany(p => p.BookingPassengers)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookingPassengers_PassengerCat");

            entity.HasOne(d => d.Gender).WithMany(p => p.BookingPassengers)
                .HasForeignKey(d => d.GenderId)
                .HasConstraintName("FK_BookingPassengers_Genders");
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__BookingS__3683B531A0C851D6");

            entity.Property(e => e.StatusId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Destination>(entity =>
        {
            entity.HasKey(e => e.DestinationId).HasName("PK__Destinat__550153910B6F1C3F");

            entity.Property(e => e.DestinationId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("destination_id");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("PK__Genders__9DF18F87F74D5300");

            entity.Property(e => e.GenderId)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("gender_id");
            entity.Property(e => e.GenderName)
                .HasMaxLength(50)
                .HasColumnName("gender_name");
        });

        modelBuilder.Entity<PassengerCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Passenge__D54EE9B49854CB65");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .HasColumnName("description");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EAEBE2E695");

            entity.Property(e => e.PaymentId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BookingId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasColumnName("method");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.StatusId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("PENDING")
                .HasColumnName("status_id");
            entity.Property(e => e.TransactionRef)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("transaction_ref");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Bookings");

            entity.HasOne(d => d.Status).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_PaymentStatuses");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__PaymentS__3683B531C1B7F85C");

            entity.Property(e => e.StatusId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__760965CC1740807B");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__783254B1E5946EFC").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.TourId).HasName("PK__Tours__4B16B9E6202F8BC0");

            entity.HasIndex(e => e.CreatedAt, "IX_Tours_CreatedAt").IsDescending();

            entity.Property(e => e.TourId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tour_id");
            entity.Property(e => e.BasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("base_price");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DestinationId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("destination_id");
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.GuideId).HasColumnName("guide_id");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.MaxParticipants).HasColumnName("max_participants");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TourCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Tours_Users");

            entity.HasOne(d => d.Destination).WithMany(p => p.Tours)
                .HasForeignKey(d => d.DestinationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tours_Destinations");

            entity.HasOne(d => d.Guide).WithMany(p => p.TourGuides)
                .HasForeignKey(d => d.GuideId)
                .HasConstraintName("FK_Tours_Guide");
        });

        modelBuilder.Entity<TourGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__TourGrou__D57795A03D9F0B61");

            entity.Property(e => e.GroupId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("group_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentBookings).HasColumnName("current_bookings");
            entity.Property(e => e.DepartDate).HasColumnName("depart_date");
            entity.Property(e => e.MaxCapacity)
                .HasDefaultValue(50)
                .HasColumnName("max_capacity");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");
            entity.Property(e => e.StatusId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("OPEN")
                .HasColumnName("status_id");
            entity.Property(e => e.TourId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tour_id");

            entity.HasOne(d => d.Status).WithMany(p => p.TourGroups)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourGroups_TourStatuses");

            entity.HasOne(d => d.Tour).WithMany(p => p.TourGroups)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourGroups_Tours");
        });

        modelBuilder.Entity<TourPrice>(entity =>
        {
            entity.HasKey(e => e.PriceId).HasName("PK__TourPric__1681726D4A4B5B8E");

            entity.Property(e => e.PriceId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("price_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(200)
                .HasColumnName("notes");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.TourId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tour_id");
            entity.Property(e => e.ValidFrom).HasColumnName("valid_from");
            entity.Property(e => e.ValidTo).HasColumnName("valid_to");

            entity.HasOne(d => d.Tour).WithMany(p => p.TourPrices)
                .HasForeignKey(d => d.TourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourPrices_Tours");
        });

        modelBuilder.Entity<TourStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__TourStat__3683B531B007CDF1");

            entity.Property(e => e.StatusId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status_id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .HasColumnName("description");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3213E83F0459C5BF");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164DF658BF1").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .HasColumnName("gender");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
