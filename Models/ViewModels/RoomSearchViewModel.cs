using HotelManagementSystem.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models.ViewModels
{
    public class RoomSearchViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime? CheckIn { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime? CheckOut { get; set; }

        [Display(Name = "Suite Type")]
        public int? RoomTypeId { get; set; }

        [Range(1, 10)]
        [Display(Name = "Guests")]
        public int? Guests { get; set; }

        [Display(Name = "Max Price per Night ($)")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Specific Facility")]
        public int? FacilityId { get; set; }

        public string? SortBy { get; set; } // "price_asc", "price_desc", "capacity"

        public IEnumerable<Room> Rooms { get; set; } = new List<Room>();
        public IEnumerable<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public IEnumerable<Facility> Facilities { get; set; } = new List<Facility>();

        // AI Match Suggestion
        public string? AiRecommendationNote { get; set; }
    }
}

