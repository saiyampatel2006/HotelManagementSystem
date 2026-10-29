using HotelManagementSystem.Models.Entities;

namespace HotelManagementSystem.Models.ViewModels
{
    public class HomeViewModel
    {
        public DateTime CheckIn { get; set; } = DateTime.Today;
        public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(1);
        public int Guests { get; set; } = 2;
        public int? RoomTypeId { get; set; }

        public IEnumerable<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public IEnumerable<Room> FeaturedRooms { get; set; } = new List<Room>();
        public IEnumerable<Facility> FeaturedFacilities { get; set; } = new List<Facility>();
        public IEnumerable<Review> RecentReviews { get; set; } = new List<Review>();

        // High-level statistics to showcase grandeur
        public int TotalLuxuryRooms { get; set; }
        public double GuestSatisfactionRating { get; set; } = 4.95;
    }
}

