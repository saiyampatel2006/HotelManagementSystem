using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using HotelManagementSystem.Models.ViewModels;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HotelManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly ApplicationDbContext _context;

        public HomeController(IRoomService roomService, ApplicationDbContext context)
        {
            _roomService = roomService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredRooms = await _roomService.GetFeaturedRoomsAsync();
            var roomTypes = await _roomService.GetAllRoomTypesAsync();
            var facilities = (await _roomService.GetAllFacilitiesAsync()).Take(6);
            var recentReviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Room)
                .OrderByDescending(r => r.CreatedAt)
                .Take(4)
                .ToListAsync();

            var totalRooms = await _context.Rooms.CountAsync();

            var model = new HomeViewModel
            {
                FeaturedRooms = featuredRooms,
                RoomTypes = roomTypes,
                FeaturedFacilities = facilities,
                RecentReviews = recentReviews,
                TotalLuxuryRooms = totalRooms,
                GuestSatisfactionRating = 4.96
            };

            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string name, string email, string phone, string message)
        {
            TempData["SuccessMessage"] = $"Thank you, {name}. Your inquiry has been routed to our Chief Concierge. We shall contact you shortly.";
            return RedirectToAction(nameof(Contact));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
