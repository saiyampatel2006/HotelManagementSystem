using HotelManagementSystem.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models.ViewModels
{
    public class RoomFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room number is required (e.g. 101, 204, V-01).")]
        [StringLength(20)]
        [Display(Name = "Room / Suite Number")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a room category.")]
        [Display(Name = "Room Category")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Price per night is required.")]
        [Range(10, 50000, ErrorMessage = "Price must be between $10 and $50,000.")]
        [Display(Name = "Nightly Tariff ($)")]
        public decimal PricePerNight { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "Capacity must be between 1 and 12.")]
        [Display(Name = "Maximum Guests")]
        public int Capacity { get; set; } = 2;

        [Required]
        [Range(1, 100)]
        [Display(Name = "Floor Level")]
        public int FloorNumber { get; set; } = 1;

        [Required]
        [Display(Name = "Current Operational Status")]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [Required(ErrorMessage = "Please provide an image URL for showcase.")]
        [Url(ErrorMessage = "Please enter a valid URL (e.g. Unsplash or hosted photo).")]
        [Display(Name = "High-Res Image URL")]
        public string ImageUrl { get; set; } = "https://images.unsplash.com/photo-1618773928121-c32242e63f39?auto=format&fit=crop&w=1200&q=80";

        [Required(ErrorMessage = "Room description is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters.")]
        [Display(Name = "Suite Description & Features")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Feature on Home Landing Page")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Selected Amenities & Facilities")]
        public List<int> SelectedFacilityIds { get; set; } = new();

        // Dropdown references
        public IEnumerable<RoomType> AvailableRoomTypes { get; set; } = new List<RoomType>();
        public IEnumerable<Facility> AvailableFacilities { get; set; } = new List<Facility>();
    }
}

