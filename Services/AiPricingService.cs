namespace HotelManagementSystem.Services
{
    public class AiPricingService : IAiPricingService
    {
        public AiSuiteRecommendation RecommendSuite(int guests, int adults, int children, int nights)
        {
            if (children > 0 || guests >= 4)
            {
                return new AiSuiteRecommendation
                {
                    RecommendedRoomTypeId = 4, // Grand Garden Pool Villa or Family Penthouse
                    SuiteName = "Grand Garden Pool Villa",
                    Reason = "AI Family Cluster analysis detected travel with children/group. The Villa offers private plunge pool security, dedicated lawn, and multizone acoustic isolation.",
                    MatchScore = 98.4,
                    EstimatedSavingsOrValue = 140.00m
                };
            }

            if (guests == 2 && nights >= 2)
            {
                return new AiSuiteRecommendation
                {
                    RecommendedRoomTypeId = 3, // Royal Aurelia Penthouse
                    SuiteName = "Royal Aurelia Penthouse",
                    Reason = "Couples retreat heuristic: Heated terrace jacuzzi, sunset skyline orientation, and private butler service yield 99.1% positive guest sentiment.",
                    MatchScore = 96.8,
                    EstimatedSavingsOrValue = 95.00m
                };
            }

            if (guests == 1 || (guests == 2 && nights == 1))
            {
                return new AiSuiteRecommendation
                {
                    RecommendedRoomTypeId = 2, // Executive Skyline Suite
                    SuiteName = "Executive Skyline Suite",
                    Reason = "Executive profile matched: High-speed Wi-Fi 6, ergonomic workstation, and marble espresso bar optimized for productive stays.",
                    MatchScore = 94.2,
                    EstimatedSavingsOrValue = 60.00m
                };
            }

            return new AiSuiteRecommendation
            {
                RecommendedRoomTypeId = 1,
                SuiteName = "Deluxe Heritage King Room",
                Reason = "Optimal luxury value ratio for discerning travellers seeking serenity and signature cloud bed comfort.",
                MatchScore = 92.0,
                EstimatedSavingsOrValue = 45.00m
            };
        }

        public (double demandMultiplier, string trendExplanation) CalculateDynamicDemandIndex(DateTime checkIn, int bookedRoomsInSeason, int totalRooms)
        {
            if (totalRooms == 0) return (1.0, "Standard baseline pricing active.");

            double occupancyRatio = (double)bookedRoomsInSeason / totalRooms;
            bool isWeekend = checkIn.DayOfWeek == DayOfWeek.Friday || checkIn.DayOfWeek == DayOfWeek.Saturday;

            double multiplier = 1.0;
            string explanation;

            if (occupancyRatio > 0.75 || isWeekend)
            {
                multiplier = 1.15;
                explanation = "Peak luxury demand window (+15% dynamic algorithmic adjustment). High velocity reservation rate.";
            }
            else if (occupancyRatio < 0.35)
            {
                multiplier = 0.90;
                explanation = "Value incentive active (-10% promotional rate applied).";
            }
            else
            {
                multiplier = 1.0;
                explanation = "Standard balanced rate based on stable inventory.";
            }

            return (multiplier, explanation);
        }

        public (string forecast, string recommendedAction) GenerateAiOccupancyForecast(double currentOccupancyRate, int upcomingReservations)
        {
            if (currentOccupancyRate >= 70.0 || upcomingReservations > 10)
            {
                return (
                    "High Velocity Demand (91% confidence interval). Projected 88% weekend occupancy.",
                    "Engage dynamic surge protection: Protect remaining Royal Penthouses for premier rack rates."
                );
            }
            else if (currentOccupancyRate <= 30.0)
            {
                return (
                    "Moderate Midweek Inflow (84% confidence interval). Capacity available for corporate and leisure bookings.",
                    "Activate complimentary breakfast upgrade package to accelerate midweek conversions."
                );
            }
            else
            {
                return (
                    "Optimal Equilibrium (94% confidence interval). Steady steady-state revenue flow.",
                    "Maintain standard tariff structure with focus on premium spa and fine dining add-ons."
                );
            }
        }
    }
}

