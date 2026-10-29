using HotelManagementSystem.Models.Entities;

namespace HotelManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int ActiveCheckIns { get; set; }
        public decimal TotalRevenue { get; set; }
        public int AvailableRoomsCount { get; set; }
        public int TotalRoomsCount { get; set; }
        public double OccupancyRate { get; set; }
        public int TotalRegisteredGuests { get; set; }

        public IEnumerable<Booking> RecentBookings { get; set; } = new List<Booking>();
        public IEnumerable<Room> HighDemandRooms { get; set; } = new List<Room>();

        // Analytics Chart Data
        public List<string> MonthlyLabels { get; set; } = new();
        public List<decimal> MonthlyRevenues { get; set; } = new();

        public List<string> RoomTypeLabels { get; set; } = new();
        public List<int> RoomTypeBookingCounts { get; set; } = new();

        // AI / ML Specialization Metrics
        public string AiOccupancyForecast { get; set; } = string.Empty;
        public string AiRecommendedPricingAction { get; set; } = string.Empty;
        public double DynamicDemandIndex { get; set; } = 1.0;
    }
}

