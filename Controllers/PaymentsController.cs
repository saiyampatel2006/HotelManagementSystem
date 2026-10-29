using HotelManagementSystem.Data;
using HotelManagementSystem.Models.ViewModels;
using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly ApplicationDbContext _context;

        public PaymentsController(IBookingService bookingService, ApplicationDbContext context)
        {
            _bookingService = bookingService;
            _context = context;
        }

        // GET: /Payments/Checkout?bookingId=1
        [HttpGet]
        public async Task<IActionResult> Checkout(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && booking.UserId != userId)
            {
                return Forbid();
            }

            // Check if already paid
            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == bookingId && p.Status == Models.Entities.PaymentStatus.Completed);

            if (existingPayment != null)
            {
                TempData["SuccessMessage"] = "This reservation has already been settled.";
                return RedirectToAction("Confirmation", "Bookings", new { reference = booking.BookingReference });
            }

            var model = new PaymentViewModel
            {
                BookingId = booking.Id,
                BookingReference = booking.BookingReference,
                GuestName = booking.User?.FullName ?? "Valued Guest",
                RoomName = booking.Room?.RoomType?.Name ?? "Suite",
                RoomNumber = booking.Room?.RoomNumber ?? "---",
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                TotalNights = booking.TotalNights,
                TotalAmount = booking.TotalPrice,
                PaymentMethod = "Credit Card",
                CardHolderName = booking.User?.FullName ?? ""
            };

            return View(model);
        }

        // POST: /Payments/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(PaymentViewModel model)
        {
            var booking = await _bookingService.GetBookingByIdAsync(model.BookingId);
            if (booking == null)
            {
                return NotFound();
            }

            var payment = await _bookingService.ProcessPaymentAsync(model);
            if (payment == null)
            {
                TempData["ErrorMessage"] = "Could not record payment transaction. Please try again.";
                return View("Checkout", model);
            }

            TempData["SuccessMessage"] = $"Payment of ${payment.Amount:N2} authorized successfully. Reference: {payment.TransactionId}";
            return RedirectToAction(nameof(Receipt), new { paymentId = payment.Id });
        }

        // GET: /Payments/Receipt?paymentId=1
        [HttpGet]
        public async Task<IActionResult> Receipt(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b!.Room)
                        .ThenInclude(r => r!.RoomType)
                .Include(p => p.Booking)
                    .ThenInclude(b => b!.User)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && payment.Booking?.UserId != userId)
            {
                return Forbid();
            }

            return View(payment);
        }
    }
}

