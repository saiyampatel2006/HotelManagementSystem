using HotelManagementSystem.Models.Entities;

namespace HotelManagementSystem.Models.ViewModels
{
    public class RoomDetailsViewModel
    {
        public Room Room { get; set; } = null!;
        public IEnumerable<Room> SimilarRooms { get; set; } = new List<Room>();
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();

        // Pre-filled dates from search
        public DateTime CheckIn { get; set; } = DateTime.Today;
        public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(1);
        public int Guests { get; set; } = 2;

        public bool IsAvailableForSelectedDates { get; set; } = true;
        public decimal EstimatedTotal { get; set; }
        public int TotalNights { get; set; } = 1;
    }
}

