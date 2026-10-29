namespace HotelManagementSystem.Services
{
    public class AiSuiteRecommendation
    {
        public int RecommendedRoomTypeId { get; set; }
        public string SuiteName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public double MatchScore { get; set; } // e.g., 96%
        public decimal EstimatedSavingsOrValue { get; set; }
    }

    public interface IAiPricingService
    {
        AiSuiteRecommendation RecommendSuite(int guests, int adults, int children, int nights);
        (double demandMultiplier, string trendExplanation) CalculateDynamicDemandIndex(DateTime checkIn, int bookedRoomsInSeason, int totalRooms);
        (string forecast, string recommendedAction) GenerateAiOccupancyForecast(double currentOccupancyRate, int upcomingReservations);
    }
}

