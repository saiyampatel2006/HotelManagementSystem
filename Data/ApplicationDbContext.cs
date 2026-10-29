using HotelManagementSystem.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RoomType> RoomTypes => Set<RoomType>();
        public DbSet<Facility> Facilities => Set<Facility>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomFacility> RoomFacilities => Set<RoomFacility>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure composite key for RoomFacility
            builder.Entity<RoomFacility>()
                .HasKey(rf => new { rf.RoomId, rf.FacilityId });

            builder.Entity<RoomFacility>()
                .HasOne(rf => rf.Room)
                .WithMany(r => r.RoomFacilities)
                .HasForeignKey(rf => rf.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RoomFacility>()
                .HasOne(rf => rf.Facility)
                .WithMany(f => f.RoomFacilities)
                .HasForeignKey(rf => rf.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Room Number
            builder.Entity<Room>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            // Unique Booking Reference
            builder.Entity<Booking>()
                .HasIndex(b => b.BookingReference)
                .IsUnique();

            // Booking to Room relation
            builder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking to User relation
            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment to Booking relation
            builder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision configurations
            builder.Entity<RoomType>()
                .Property(rt => rt.BasePrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Room>()
                .Property(r => r.PricePerNight)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Booking>()
                .Property(b => b.SubTotal)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Booking>()
                .Property(b => b.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}

