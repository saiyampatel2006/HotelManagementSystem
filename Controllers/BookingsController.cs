using HotelManagementSystem.Models.Entities;
using HotelManagementSystem.Models.ViewModels;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;

        public BookingsController(IBookingService bookingService, IRoomService roomService)
        {
            _bookingService = bookingService;
            _roomService = roomService;
        }

        // GET: /Bookings/Create?roomId=1&checkIn=...&checkOut=...&guests=...
        [HttpGet]
        public async Task<IActionResult> Create(int roomId, DateTime? checkIn, DateTime? checkOut, int? guests)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            if (room == null)
            {
                TempData["ErrorMessage"] = "Selected room not found.";
                return RedirectToAction("Index", "Rooms");
            }

            var cin = checkIn ?? DateTime.Today;
            var cout = checkOut ?? DateTime.Today.AddDays(1);
            if (cout <= cin) cout = cin.AddDays(1);

            int guestCount = guests ?? 2;
            if (guestCount > room.Capacity) guestCount = room.Capacity;

            var model = new BookingCreateViewModel
            {
                RoomId = room.Id,
                RoomNumber = room.RoomNumber,
                RoomTypeName = room.RoomType?.Name ?? "Luxury Suite",
                PricePerNight = room.PricePerNight,
                RoomImageUrl = room.ImageUrl,
                RoomCapacity = room.Capacity,
                CheckInDate = cin,
                CheckOutDate = cout,
                Adults = guestCount,
                Children = 0
            };

            return View(model);
        }

        // POST: /Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            var room = await _roomService.GetRoomByIdAsync(model.RoomId);
            if (room == null)
            {
                TempData["ErrorMessage"] = "The requested suite was not found.";
                return RedirectToAction("Index", "Rooms");
            }

            // Repopulate display fields in case of validation failure
            model.RoomNumber = room.RoomNumber;
            model.RoomTypeName = room.RoomType?.Name ?? "Luxury Suite";
            model.PricePerNight = room.PricePerNight;
            model.RoomImageUrl = room.ImageUrl;
            model.RoomCapacity = room.Capacity;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Server-side check for overlapping reservations
            bool isAvailable = await _bookingService.IsRoomAvailableAsync(model.RoomId, model.CheckInDate, model.CheckOutDate);
            if (!isAvailable)
            {
                ModelState.AddModelError(string.Empty,
                    $"Room {room.RoomNumber} is already reserved for the chosen dates ({model.CheckInDate:yyyy-MM-dd} to {model.CheckOutDate:yyyy-MM-dd}). Please select other dates.");
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _bookingService.CreateBookingAsync(model, userId);

            if (!result.Success || result.Booking == null)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Could not complete booking.");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Your reservation has been initiated! Please complete your demo payment to confirm.";
            return RedirectToAction("Checkout", "Payments", new { bookingId = result.Booking.Id });
        }

        // GET: /Bookings/Confirmation?reference=AUR-123456
        [HttpGet]
        public async Task<IActionResult> Confirmation(string reference)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _bookingService.GetBookingByReferenceAsync(reference, User.IsInRole("Admin") ? null : userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: /Bookings/MyBookings
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return View(bookings);
        }

        // GET: /Bookings/Details?reference=AUR-123456
        [HttpGet]
        public async Task<IActionResult> Details(string reference)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _bookingService.GetBookingByReferenceAsync(reference, User.IsInRole("Admin") ? null : userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: /Bookings/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                reason = "Guest requested cancellation via online portal.";
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isAdmin = User.IsInRole("Admin");

            var success = await _bookingService.CancelBookingAsync(id, reason, userId, isAdmin);

            if (success)
            {
                TempData["SuccessMessage"] = "Your reservation has been cancelled. Any processed payment will be refunded according to our luxury guest guarantee.";
            }
            else
            {
                TempData["ErrorMessage"] = "This reservation could not be cancelled. Only active upcoming bookings are eligible for online cancellation.";
            }

            if (isAdmin)
            {
                return RedirectToAction("Bookings", "Admin");
            }

            return RedirectToAction(nameof(MyBookings));
        }
    }
}

