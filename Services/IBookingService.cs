using HotelManagementSystem.Models.Entities;
using HotelManagementSystem.Models.ViewModels;

namespace HotelManagementSystem.Services
{
    public class BookingResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Booking? Booking { get; set; }
    }

    public interface IBookingService
    {
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null);
        Task<BookingResult> CreateBookingAsync(BookingCreateViewModel model, string userId);
        Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId);
        Task<Booking?> GetBookingByReferenceAsync(string reference, string? userId = null);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetAllBookingsAsync(BookingStatus? status = null, string? search = null);
        Task<bool> CancelBookingAsync(int id, string reason, string? userId = null, bool isAdmin = false);
        Task<bool> CheckInGuestAsync(int id);
        Task<bool> CheckOutGuestAsync(int id);
        Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status);
        Task<Payment?> ProcessPaymentAsync(PaymentViewModel model);
        Task<DashboardViewModel> GetDashboardStatsAsync();
        Task<ReportsViewModel> GetReportsAsync(DateTime startDate, DateTime endDate);
    }
}

