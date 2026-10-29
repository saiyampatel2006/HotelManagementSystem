using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementSystem.Models.Entities
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Refunded,
        Failed
    }

    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        public virtual Booking? Booking { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Payment Amount ($)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Credit Card"; // "Credit Card", "Debit Card", "UPI / QR Code", "Net Banking", "Cash on Arrival"

        [Required]
        [MaxLength(100)]
        [Display(Name = "Transaction Reference / ID")]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Payment Status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

        [Display(Name = "Payment Timestamp")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [MaxLength(250)]
        public string? Notes { get; set; }
    }
}

