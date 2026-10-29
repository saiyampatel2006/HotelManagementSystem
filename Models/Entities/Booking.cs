using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementSystem.Models.Entities
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        CheckedIn,
        CheckedOut,
        Cancelled
    }

    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        [Display(Name = "Booking Code")]
        public string BookingReference { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [Display(Name = "Room")]
        public int RoomId { get; set; }
        public virtual Room? Room { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 90)]
        [Display(Name = "Total Nights")]
        public int TotalNights { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Room Charges")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Taxes & Luxury Cess (12%)")]
        public decimal TaxAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Range(1, 10)]
        [Display(Name = "Adult Guests")]
        public int Adults { get; set; } = 1;

        [Range(0, 8)]
        [Display(Name = "Children")]
        public int Children { get; set; } = 0;

        [MaxLength(500)]
        [Display(Name = "Special Requests / Dietary Needs")]
        public string? SpecialRequests { get; set; }

        [Required]
        [Display(Name = "Reservation Status")]
        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ActualCheckInTime { get; set; }
        public DateTime? ActualCheckOutTime { get; set; }

        [MaxLength(250)]
        [Display(Name = "Cancellation Reason")]
        public string? CancellationReason { get; set; }

        public DateTime? CancelledAt { get; set; }

        // Navigation
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

