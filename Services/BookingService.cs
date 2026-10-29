using HotelManagementSystem.Data;
using HotelManagementSystem.Models.Entities;
using HotelManagementSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAiPricingService _aiPricingService;

        public BookingService(ApplicationDbContext context, IAiPricingService aiPricingService)
        {
            _context = context;
            _aiPricingService = aiPricingService;
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut, int? excludeBookingId = null)
        {
            var cin = checkIn.Date;
            var cout = checkOut.Date;

            if (cout <= cin) return false;

            var query = _context.Bookings
                .Where(b => b.RoomId == roomId && b.Status != BookingStatus.Cancelled);

            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookingId.Value);
            }

            // Overlap condition:
            // Existing booking starts before requested checkout, AND ends after requested checkin
            bool isOverlapping = await query.AnyAsync(b =>
                cin < b.CheckOutDate.Date && cout > b.CheckInDate.Date);

            return !isOverlapping;
        }

        public async Task<BookingResult> CreateBookingAsync(BookingCreateViewModel model, string userId)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == model.RoomId);

            if (room == null)
            {
                return new BookingResult { Success = false, ErrorMessage = "Requested suite could not be found." };
            }

            if (room.Status == RoomStatus.Maintenance)
            {
                return new BookingResult { Success = false, ErrorMessage = "This suite is currently undergoing scheduled maintenance." };
            }

            // Validate non-overlapping reservation
            bool isAvailable = await IsRoomAvailableAsync(model.RoomId, model.CheckInDate, model.CheckOutDate);
            if (!isAvailable)
            {
                return new BookingResult
                {
                    Success = false,
                    ErrorMessage = $"Room {room.RoomNumber} is already booked for the selected dates ({model.CheckInDate:MMM dd, yyyy} to {model.CheckOutDate:MMM dd, yyyy}). Please select alternative dates or browse our other suites."
                };
            }

            int nights = Math.Max(1, (model.CheckOutDate.Date - model.CheckInDate.Date).Days);
            decimal subTotal = room.PricePerNight * nights;
            decimal taxAmount = Math.Round(subTotal * 0.12m, 2);
            decimal totalPrice = subTotal + taxAmount;

            var randomSuffix = Random.Shared.Next(100000, 999999);
            var bookingRef = $"AUR-{randomSuffix}";

            var booking = new Booking
            {
                BookingReference = bookingRef,
                UserId = userId,
                RoomId = room.Id,
                CheckInDate = model.CheckInDate.Date,
                CheckOutDate = model.CheckOutDate.Date,
                TotalNights = nights,
                SubTotal = subTotal,
                TaxAmount = taxAmount,
                TotalPrice = totalPrice,
                Adults = model.Adults,
                Children = model.Children,
                SpecialRequests = model.SpecialRequests?.Trim(),
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            return new BookingResult
            {
                Success = true,
                Booking = booking
            };
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomType)
                .Include(b => b.Payments)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByReferenceAsync(string reference, string? userId = null)
        {
            var query = _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomType)
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomFacilities)
                        .ThenInclude(rf => rf.Facility)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .Where(b => b.BookingReference.ToUpper() == reference.Trim().ToUpper());

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(b => b.UserId == userId);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomType)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync(BookingStatus? status = null, string? search = null)
        {
            var query = _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r!.RoomType)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(b =>
                    b.BookingReference.ToLower().Contains(search) ||
                    (b.User != null && b.User.FullName.ToLower().Contains(search)) ||
                    (b.User != null && b.User.Email != null && b.User.Email.ToLower().Contains(search)) ||
                    (b.Room != null && b.Room.RoomNumber.ToLower().Contains(search)));
            }

            return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        }

        public async Task<bool> CancelBookingAsync(int id, string reason, string? userId = null, bool isAdmin = false)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return false;

            // Security check: non-admin can only cancel their own reservation
            if (!isAdmin && booking.UserId != userId)
            {
                return false;
            }

            // Cannot cancel already completed or in-progress stays
            if (booking.Status == BookingStatus.CheckedIn || booking.Status == BookingStatus.CheckedOut)
            {
                return false;
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                return true;
            }

            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = reason.Trim();
            booking.CancelledAt = DateTime.UtcNow;

            // Adjust payment records if any
            foreach (var payment in booking.Payments.Where(p => p.Status == PaymentStatus.Completed))
            {
                payment.Status = PaymentStatus.Refunded;
                payment.Notes = (payment.Notes ?? "") + $" [Refund initiated upon cancellation on {DateTime.UtcNow:yyyy-MM-dd}]";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckInGuestAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return false;

            booking.Status = BookingStatus.CheckedIn;
            booking.ActualCheckInTime = DateTime.UtcNow;

            if (booking.Room != null)
            {
                booking.Room.Status = RoomStatus.Booked;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckOutGuestAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return false;

            booking.Status = BookingStatus.CheckedOut;
            booking.ActualCheckOutTime = DateTime.UtcNow;

            if (booking.Room != null)
            {
                // After checkout, room moves to Cleaning so housekeeping can prepare it
                booking.Room.Status = RoomStatus.Cleaning;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBookingStatusAsync(int id, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            booking.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Payment?> ProcessPaymentAsync(PaymentViewModel model)
        {
            var booking = await _context.Bookings.FindAsync(model.BookingId);
            if (booking == null) return null;

            var txnPrefix = model.PaymentMethod.Contains("UPI") ? "TXN-UPI-" : "TXN-CC-";
            var txnId = $"{txnPrefix}{Random.Shared.Next(10000000, 99999999)}";

            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = model.TotalAmount > 0 ? model.TotalAmount : booking.TotalPrice,
                PaymentMethod = model.PaymentMethod,
                TransactionId = txnId,
                Status = PaymentStatus.Completed,
                PaymentDate = DateTime.UtcNow,
                Notes = string.IsNullOrWhiteSpace(model.Notes)
                    ? $"Successful authorization via {model.PaymentMethod}."
                    : model.Notes.Trim()
            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<DashboardViewModel> GetDashboardStatsAsync()
        {
            var totalBookings = await _context.Bookings.CountAsync();
            var activeCheckIns = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.CheckedIn);

            var totalRevenue = (decimal)(await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled)
                .SumAsync(b => (double)b.TotalPrice));

            var totalRooms = await _context.Rooms.CountAsync();
            var availableRooms = await _context.Rooms.CountAsync(r => r.Status == RoomStatus.Available);

            double occupancyRate = totalRooms > 0
                ? Math.Round(((double)(totalRooms - availableRooms) / totalRooms) * 100.0, 1)
                : 0.0;

            var totalGuests = await _context.Users.CountAsync();

            var recentBookings = await _context.Bookings
                .Include(b => b.Room).ThenInclude(r => r!.RoomType)
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .Take(7)
                .ToListAsync();

            // Monthly revenue breakdown (last 6 months)
            var monthlyLabels = new List<string>();
            var monthlyRevenues = new List<decimal>();

            var now = DateTime.UtcNow;
            for (int i = 5; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                var sumDouble = await _context.Bookings
                    .Where(b => b.Status != BookingStatus.Cancelled && b.CreatedAt >= monthStart && b.CreatedAt < monthEnd)
                    .SumAsync(b => (double?)b.TotalPrice) ?? 0.0;

                monthlyLabels.Add(monthDate.ToString("MMM yyyy"));
                monthlyRevenues.Add((decimal)sumDouble);
            }

            // Room Type popularity breakdown
            var roomTypes = await _context.RoomTypes
                .Include(rt => rt.Rooms)
                    .ThenInclude(r => r.Bookings)
                .ToListAsync();

            var roomTypeLabels = new List<string>();
            var roomTypeCounts = new List<int>();

            foreach (var rt in roomTypes)
            {
                roomTypeLabels.Add(rt.Name);
                int count = rt.Rooms.SelectMany(r => r.Bookings).Count(b => b.Status != BookingStatus.Cancelled);
                roomTypeCounts.Add(count);
            }

            // AI / ML Specialization Metrics
            var (forecast, recommendedAction) = _aiPricingService.GenerateAiOccupancyForecast(occupancyRate, totalBookings);
            var (demandIndex, _) = _aiPricingService.CalculateDynamicDemandIndex(DateTime.Today, totalRooms - availableRooms, totalRooms);

            return new DashboardViewModel
            {
                TotalBookings = totalBookings,
                ActiveCheckIns = activeCheckIns,
                TotalRevenue = totalRevenue,
                TotalRoomsCount = totalRooms,
                AvailableRoomsCount = availableRooms,
                OccupancyRate = occupancyRate,
                TotalRegisteredGuests = totalGuests,
                RecentBookings = recentBookings,
                MonthlyLabels = monthlyLabels,
                MonthlyRevenues = monthlyRevenues,
                RoomTypeLabels = roomTypeLabels,
                RoomTypeBookingCounts = roomTypeCounts,
                AiOccupancyForecast = forecast,
                AiRecommendedPricingAction = recommendedAction,
                DynamicDemandIndex = demandIndex
            };
        }

        public async Task<ReportsViewModel> GetReportsAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1).AddTicks(-1);

            var bookings = await _context.Bookings
                .Include(b => b.Room).ThenInclude(r => r!.RoomType)
                .Include(b => b.User)
                .Include(b => b.Payments)
                .Where(b => b.CreatedAt >= start && b.CreatedAt <= end)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var totalBookings = bookings.Count;
            var confirmed = bookings.Count(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn);
            var completed = bookings.Count(b => b.Status == BookingStatus.CheckedOut);
            var cancelled = bookings.Count(b => b.Status == BookingStatus.Cancelled);

            var gross = bookings.Where(b => b.Status != BookingStatus.Cancelled).Sum(b => b.TotalPrice);
            var tax = bookings.Where(b => b.Status != BookingStatus.Cancelled).Sum(b => b.TaxAmount);
            var net = gross - tax;

            // Category breakdown
            var categories = await _context.RoomTypes.ToListAsync();
            var categoryBreakdown = new List<CategoryRevenueItem>();

            foreach (var cat in categories)
            {
                var catBookings = bookings.Where(b => b.Room?.RoomTypeId == cat.Id && b.Status != BookingStatus.Cancelled).ToList();
                decimal catRevenue = catBookings.Sum(b => b.TotalPrice);
                int catNights = catBookings.Sum(b => b.TotalNights);

                categoryBreakdown.Add(new CategoryRevenueItem
                {
                    CategoryName = cat.Name,
                    BookingCount = catBookings.Count,
                    TotalNightsBooked = catNights,
                    TotalRevenue = catRevenue,
                    PercentageOfRevenue = gross > 0 ? Math.Round((double)(catRevenue / gross) * 100.0, 1) : 0.0
                });
            }

            return new ReportsViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalBookings = totalBookings,
                ConfirmedBookings = confirmed,
                CompletedStays = completed,
                CancelledBookings = cancelled,
                GrossRevenue = gross,
                TaxCollected = tax,
                NetRevenue = net,
                CategoryBreakdown = categoryBreakdown,
                ReportBookings = bookings
            };
        }
    }
}
