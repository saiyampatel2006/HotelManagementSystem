using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models.ViewModels
{
    public class PaymentViewModel
    {
        [Required]
        public int BookingId { get; set; }

        public string BookingReference { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Please select a payment method.")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Credit Card";

        // Demo Card Details
        [Display(Name = "Name on Card")]
        public string? CardHolderName { get; set; }

        [Display(Name = "Card Number (16 Digits)")]
        [RegularExpression(@"^(\d{4}[- ]?){3}\d{4}$", ErrorMessage = "Please enter a valid 16-digit card number.")]
        public string? CardNumber { get; set; }

        [Display(Name = "Expiry (MM/YY)")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Please enter expiry as MM/YY.")]
        public string? CardExpiry { get; set; }

        [Display(Name = "CVV")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
        public string? CardCvv { get; set; }

        // Demo UPI Details
        [Display(Name = "UPI ID / VPA")]
        public string? UpiId { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }
    }
}

