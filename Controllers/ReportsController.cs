using HotelManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HotelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IBookingService _bookingService;

        public ReportsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: /Reports
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today.AddDays(30);

            if (end < start) end = start.AddDays(30);

            var report = await _bookingService.GetReportsAsync(start, end);
            return View(report);
        }

        // GET: /Reports/ExportCsv
        public async Task<IActionResult> ExportCsv(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today.AddDays(30);

            var report = await _bookingService.GetReportsAsync(start, end);

            var sb = new StringBuilder();
            sb.AppendLine("BookingReference,GuestName,GuestEmail,RoomNumber,RoomType,CheckIn,CheckOut,Nights,SubTotal,Taxes,TotalAmount,Status,CreatedAt");

            foreach (var b in report.ReportBookings)
            {
                sb.AppendLine($"\"{b.BookingReference}\",\"{b.User?.FullName}\",\"{b.User?.Email}\",\"{b.Room?.RoomNumber}\",\"{b.Room?.RoomType?.Name}\",\"{b.CheckInDate:yyyy-MM-dd}\",\"{b.CheckOutDate:yyyy-MM-dd}\",{b.TotalNights},{b.SubTotal:F2},{b.TaxAmount:F2},{b.TotalPrice:F2},\"{b.Status}\",\"{b.CreatedAt:yyyy-MM-dd HH:mm}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"GrandAurelia_RevenueReport_{start:yyyyMMdd}_to_{end:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }
    }
}

