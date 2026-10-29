using HotelManagementSystem.Models.ViewModels;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;
        private readonly IAiPricingService _aiPricingService;

        public RoomsController(
            IRoomService roomService,
            IBookingService bookingService,
            IAiPricingService aiPricingService)
        {
            _roomService = roomService;
            _bookingService = bookingService;
            _aiPricingService = aiPricingService;
        }

        public async Task<IActionResult> Index(RoomSearchViewModel search)
        {
            // Default dates if partially provided
            if (search.CheckIn.HasValue && !search.CheckOut.HasValue)
            {
                search.CheckOut = search.CheckIn.Value.AddDays(1);
            }

            var rooms = await _roomService.GetAllRoomsAsync(
                search.RoomTypeId,
                search.Guests,
                search.MaxPrice,
                search.CheckIn,
                search.CheckOut,
                search.FacilityId,
                search.SortBy);

            search.Rooms = rooms;
            search.RoomTypes = await _roomService.GetAllRoomTypesAsync();
            search.Facilities = await _roomService.GetAllFacilitiesAsync();

            // AI Recommendation logic for AIML evaluation
            if (search.Guests.HasValue && search.Guests.Value > 0)
            {
                int nights = (search.CheckIn.HasValue && search.CheckOut.HasValue)
                    ? Math.Max(1, (search.CheckOut.Value.Date - search.CheckIn.Value.Date).Days)
                    : 1;

                var recommendation = _aiPricingService.RecommendSuite(search.Guests.Value, search.Guests.Value, 0, nights);
                search.AiRecommendationNote = $"AI Curated Match ({recommendation.MatchScore}% Match): {recommendation.Reason}";
            }

            return View(search);
        }

        public async Task<IActionResult> Details(int id, DateTime? checkIn, DateTime? checkOut, int? guests)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var cin = checkIn ?? DateTime.Today;
            var cout = checkOut ?? DateTime.Today.AddDays(1);
            if (cout <= cin) cout = cin.AddDays(1);

            int guestCount = guests ?? 2;
            int nights = (cout.Date - cin.Date).Days;
            decimal estimatedTotal = (room.PricePerNight * nights) * 1.12m;

            bool isAvailable = await _bookingService.IsRoomAvailableAsync(room.Id, cin, cout);

            var similarRooms = await _roomService.GetSimilarRoomsAsync(room.Id, room.RoomTypeId);
            var reviews = await _roomService.GetRoomReviewsAsync(room.Id);

            var model = new RoomDetailsViewModel
            {
                Room = room,
                SimilarRooms = similarRooms,
                Reviews = reviews,
                CheckIn = cin,
                CheckOut = cout,
                Guests = guestCount,
                TotalNights = nights,
                EstimatedTotal = estimatedTotal,
                IsAvailableForSelectedDates = isAvailable
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int roomId, int rating, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["ErrorMessage"] = "Please write a brief feedback comment.";
                return RedirectToAction(nameof(Details), new { id = roomId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            await _roomService.AddReviewAsync(roomId, userId, rating, comment);
            TempData["SuccessMessage"] = "Thank you! Your feedback has been published.";
            return RedirectToAction(nameof(Details), new { id = roomId });
        }
    }
}

