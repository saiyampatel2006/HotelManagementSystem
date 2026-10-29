using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementSystem.Models.Entities
{
    public enum RoomStatus
    {
        Available,
        Booked,
        Maintenance,
        Cleaning
    }

    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }
        public virtual RoomType? RoomType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(10, 50000)]
        [Display(Name = "Price Per Night ($)")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(1, 10)]
        [Display(Name = "Max Guests")]
        public int Capacity { get; set; } = 2;

        [Required]
        [Display(Name = "Room Status")]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [Required]
        [MaxLength(500)]
        [Display(Name = "Photo URL")]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 100)]
        [Display(Name = "Floor")]
        public int FloorNumber { get; set; } = 1;

        public bool IsFeatured { get; set; } = false;

        // Navigation
        public virtual ICollection<RoomFacility> RoomFacilities { get; set; } = new List<RoomFacility>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

