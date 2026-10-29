using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models.ViewModels
{
    public class BookingCreateViewModel : IValidatableObject
    {
        [Required]
        public int RoomId { get; set; }

        public string? RoomNumber { get; set; }
        public string? RoomTypeName { get; set; }
        public decimal PricePerNight { get; set; }
        public string? RoomImageUrl { get; set; }
        public int RoomCapacity { get; set; }

        [Required(ErrorMessage = "Please select a check-in date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select a check-out date.")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        [Range(1, 10, ErrorMessage = "Please select between 1 and 10 adult guests.")]
        [Display(Name = "Adult Guests")]
        public int Adults { get; set; } = 2;

        [Range(0, 8, ErrorMessage = "Children count must be between 0 and 8.")]
        [Display(Name = "Children (under 12)")]
        public int Children { get; set; } = 0;

        [MaxLength(500, ErrorMessage = "Special requests cannot exceed 500 characters.")]
        [Display(Name = "Special Requests / Airport Transfer / Dietary Preferences")]
        public string? SpecialRequests { get; set; }

        // Computed
        public int TotalNights => Math.Max(1, (CheckOutDate.Date - CheckInDate.Date).Days);
        public decimal SubTotal => PricePerNight * TotalNights;
        public decimal TaxAmount => SubTotal * 0.12m;
        public decimal TotalPrice => SubTotal + TaxAmount;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckInDate.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "Check-In date cannot be in the past.",
                    [nameof(CheckInDate)]);
            }

            if (CheckOutDate.Date <= CheckInDate.Date)
            {
                yield return new ValidationResult(
                    "Check-Out date must be at least one day after Check-In date.",
                    [nameof(CheckOutDate)]);
            }

            if ((CheckOutDate.Date - CheckInDate.Date).TotalDays > 60)
            {
                yield return new ValidationResult(
                    "Maximum allowable continuous reservation is 60 nights.",
                    [nameof(CheckOutDate)]);
            }

            if (RoomCapacity > 0 && (Adults + Children) > RoomCapacity)
            {
                yield return new ValidationResult(
                    $"The total number of guests ({Adults + Children}) exceeds this suite's maximum capacity of {RoomCapacity} guests.",
                    [nameof(Adults)]);
            }
        }
    }
}

