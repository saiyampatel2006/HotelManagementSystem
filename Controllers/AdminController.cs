using HotelManagementSystem.Data;
using HotelManagementSystem.Models.Entities;
using HotelManagementSystem.Models.ViewModels;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            IBookingService bookingService,
            IRoomService roomService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _context = context;
            _userManager = userManager;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            var dashboardData = await _bookingService.GetDashboardStatsAsync();
            return View(dashboardData);
        }

        // GET: /Admin/Rooms
        public async Task<IActionResult> Rooms(int? roomTypeId, RoomStatus? status, string? search)
        {
            var query = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomFacilities).ThenInclude(rf => rf.Facility)
                .AsQueryable();

            if (roomTypeId.HasValue && roomTypeId.Value > 0)
            {
                query = query.Where(r => r.RoomTypeId == roomTypeId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(r => r.RoomNumber.ToLower().Contains(search) ||
                                         (r.RoomType != null && r.RoomType.Name.ToLower().Contains(search)));
            }

            var rooms = await query.OrderBy(r => r.RoomNumber).ToListAsync();
            ViewBag.RoomTypes = await _roomService.GetAllRoomTypesAsync();
            ViewBag.SelectedType = roomTypeId;
            ViewBag.SelectedStatus = status;
            ViewBag.SearchTerm = search;

            return View(rooms);
        }

        // GET: /Admin/CreateRoom
        [HttpGet]
        public async Task<IActionResult> CreateRoom()
        {
            var model = new RoomFormViewModel
            {
                AvailableRoomTypes = await _roomService.GetAllRoomTypesAsync(),
                AvailableFacilities = await _roomService.GetAllFacilitiesAsync()
            };
            return View(model);
        }

        // POST: /Admin/CreateRoom
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom(RoomFormViewModel model)
        {
            if (await _roomService.RoomNumberExistsAsync(model.RoomNumber))
            {
                ModelState.AddModelError(nameof(model.RoomNumber), $"Room number '{model.RoomNumber}' is already registered.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoomTypes = await _roomService.GetAllRoomTypesAsync();
                model.AvailableFacilities = await _roomService.GetAllFacilitiesAsync();
                return View(model);
            }

            var room = await _roomService.CreateRoomAsync(model);
            TempData["SuccessMessage"] = $"Room #{room.RoomNumber} was added successfully to hotel inventory.";
            return RedirectToAction(nameof(Rooms));
        }

        // GET: /Admin/EditRoom/5
        [HttpGet]
        public async Task<IActionResult> EditRoom(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();

            var model = new RoomFormViewModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomTypeId = room.RoomTypeId,
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                FloorNumber = room.FloorNumber,
                Status = room.Status,
                ImageUrl = room.ImageUrl,
                Description = room.Description,
                IsFeatured = room.IsFeatured,
                SelectedFacilityIds = room.RoomFacilities.Select(rf => rf.FacilityId).ToList(),
                AvailableRoomTypes = await _roomService.GetAllRoomTypesAsync(),
                AvailableFacilities = await _roomService.GetAllFacilitiesAsync()
            };

            return View(model);
        }

        // POST: /Admin/EditRoom/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(RoomFormViewModel model)
        {
            if (await _roomService.RoomNumberExistsAsync(model.RoomNumber, model.Id))
            {
                ModelState.AddModelError(nameof(model.RoomNumber), $"Room number '{model.RoomNumber}' is assigned to another room.");
            }

            if (!ModelState.IsValid)
            {
                model.AvailableRoomTypes = await _roomService.GetAllRoomTypesAsync();
                model.AvailableFacilities = await _roomService.GetAllFacilitiesAsync();
                return View(model);
            }

            var updated = await _roomService.UpdateRoomAsync(model);
            if (updated == null) return NotFound();

            TempData["SuccessMessage"] = $"Room #{updated.RoomNumber} specifications updated successfully.";
            return RedirectToAction(nameof(Rooms));
        }

        // POST: /Admin/DeleteRoom/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var deleted = await _roomService.DeleteRoomAsync(id);
            if (deleted)
            {
                TempData["SuccessMessage"] = "Room was deleted from inventory.";
            }
            else
            {
                TempData["ErrorMessage"] = "Cannot permanently delete a room with active or upcoming reservations. The room status was automatically set to 'Maintenance'.";
            }

            return RedirectToAction(nameof(Rooms));
        }

        // POST: /Admin/SetRoomStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRoomStatus(int id, RoomStatus status)
        {
            var success = await _roomService.UpdateRoomStatusAsync(id, status);
            if (success)
            {
                TempData["SuccessMessage"] = $"Room status changed to '{status}'.";
            }
            return RedirectToAction(nameof(Rooms));
        }

        // GET: /Admin/Bookings
        public async Task<IActionResult> Bookings(BookingStatus? status, string? search)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(status, search);
            ViewBag.SelectedStatus = status;
            ViewBag.SearchTerm = search;
            return View(bookings);
        }

        // GET: /Admin/BookingDetails/5
        public async Task<IActionResult> BookingDetails(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: /Admin/CheckInGuest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckInGuest(int id)
        {
            var success = await _bookingService.CheckInGuestAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Guest successfully checked in. Keycards issued and room marked as Booked.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to process check-in.";
            }
            return RedirectToAction(nameof(BookingDetails), new { id });
        }

        // POST: /Admin/CheckOutGuest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutGuest(int id)
        {
            var success = await _bookingService.CheckOutGuestAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Guest checked out. Room marked for Housekeeping / Cleaning.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to process check-out.";
            }
            return RedirectToAction(nameof(BookingDetails), new { id });
        }

        // POST: /Admin/CancelBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id, string reason)
        {
            var success = await _bookingService.CancelBookingAsync(id, reason, isAdmin: true);
            if (success)
            {
                TempData["SuccessMessage"] = "Reservation was cancelled by management and refundable credits processed.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not cancel reservation.";
            }
            return RedirectToAction(nameof(BookingDetails), new { id });
        }

        // GET: /Admin/Customers
        public async Task<IActionResult> Customers()
        {
            var customers = await _context.Users
                .Include(u => u.Bookings)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(customers);
        }
    }
}

