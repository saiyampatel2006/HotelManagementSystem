using HotelManagementSystem.Data;
using HotelManagementSystem.Models.Entities;
using HotelManagementSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Services
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync(
            int? roomTypeId = null,
            int? guests = null,
            decimal? maxPrice = null,
            DateTime? checkIn = null,
            DateTime? checkOut = null,
            int? facilityId = null,
            string? sortBy = null)
        {
            var query = _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomFacilities)
                    .ThenInclude(rf => rf.Facility)
                .Include(r => r.Bookings)
                .AsQueryable();

            if (roomTypeId.HasValue && roomTypeId.Value > 0)
            {
                query = query.Where(r => r.RoomTypeId == roomTypeId.Value);
            }

            if (guests.HasValue && guests.Value > 0)
            {
                query = query.Where(r => r.Capacity >= guests.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(r => r.PricePerNight <= maxPrice.Value);
            }

            if (facilityId.HasValue && facilityId.Value > 0)
            {
                query = query.Where(r => r.RoomFacilities.Any(rf => rf.FacilityId == facilityId.Value));
            }

            // Exclude rooms with overlapping bookings if check-in & check-out dates are supplied
            if (checkIn.HasValue && checkOut.HasValue && checkOut.Value > checkIn.Value)
            {
                var cin = checkIn.Value.Date;
                var cout = checkOut.Value.Date;

                query = query.Where(r => !r.Bookings.Any(b =>
                    b.Status != BookingStatus.Cancelled &&
                    cin < b.CheckOutDate.Date &&
                    cout > b.CheckInDate.Date));
            }

            // Sorting
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(r => (double)r.PricePerNight),
                "price_desc" => query.OrderByDescending(r => (double)r.PricePerNight),
                "capacity" => query.OrderByDescending(r => r.Capacity),
                _ => query.OrderBy(r => r.RoomNumber)
            };

            return await query.ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.RoomFacilities)
                    .ThenInclude(rf => rf.Facility)
                .Include(r => r.Reviews)
                    .ThenInclude(rev => rev.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Room>> GetFeaturedRoomsAsync()
        {
            return await _context.Rooms
                .Where(r => r.IsFeatured && r.Status == RoomStatus.Available)
                .OrderBy(r => r.RoomNumber)
                .Include(r => r.RoomType)
                .Include(r => r.RoomFacilities)
                    .ThenInclude(rf => rf.Facility)
                .Take(4)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomType>> GetAllRoomTypesAsync()
        {
            return await _context.RoomTypes
                .Include(rt => rt.Rooms)
                .OrderBy(rt => (double)rt.BasePrice)
                .ToListAsync();
        }

        public async Task<IEnumerable<Facility>> GetAllFacilitiesAsync()
        {
            return await _context.Facilities
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        public async Task<Room> CreateRoomAsync(RoomFormViewModel model)
        {
            var room = new Room
            {
                RoomNumber = model.RoomNumber.Trim(),
                RoomTypeId = model.RoomTypeId,
                PricePerNight = model.PricePerNight,
                Capacity = model.Capacity,
                FloorNumber = model.FloorNumber,
                Status = model.Status,
                ImageUrl = model.ImageUrl.Trim(),
                Description = model.Description.Trim(),
                IsFeatured = model.IsFeatured
            };

            if (model.SelectedFacilityIds.Any())
            {
                foreach (var fid in model.SelectedFacilityIds)
                {
                    room.RoomFacilities.Add(new RoomFacility
                    {
                        FacilityId = fid
                    });
                }
            }

            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room?> UpdateRoomAsync(RoomFormViewModel model)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomFacilities)
                .FirstOrDefaultAsync(r => r.Id == model.Id);

            if (room == null) return null;

            room.RoomNumber = model.RoomNumber.Trim();
            room.RoomTypeId = model.RoomTypeId;
            room.PricePerNight = model.PricePerNight;
            room.Capacity = model.Capacity;
            room.FloorNumber = model.FloorNumber;
            room.Status = model.Status;
            room.ImageUrl = model.ImageUrl.Trim();
            room.Description = model.Description.Trim();
            room.IsFeatured = model.IsFeatured;

            // Synchronize facilities
            room.RoomFacilities.Clear();
            if (model.SelectedFacilityIds.Any())
            {
                foreach (var fid in model.SelectedFacilityIds)
                {
                    room.RoomFacilities.Add(new RoomFacility
                    {
                        RoomId = room.Id,
                        FacilityId = fid
                    });
                }
            }

            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return false;

            // Check if there are active non-cancelled bookings
            bool hasActiveBookings = room.Bookings.Any(b =>
                b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn);

            if (hasActiveBookings)
            {
                // Can't hard delete room with active bookings; set status to Maintenance
                room.Status = RoomStatus.Maintenance;
                await _context.SaveChangesAsync();
                return false;
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRoomStatusAsync(int roomId, RoomStatus status)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return false;

            room.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RoomNumberExistsAsync(string roomNumber, int? excludeId = null)
        {
            var query = _context.Rooms.Where(r => r.RoomNumber.ToLower() == roomNumber.Trim().ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Room>> GetSimilarRoomsAsync(int roomId, int roomTypeId, int take = 3)
        {
            return await _context.Rooms
                .Where(r => r.Id != roomId && r.RoomTypeId == roomTypeId)
                .Include(r => r.RoomType)
                .Include(r => r.RoomFacilities).ThenInclude(rf => rf.Facility)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetRoomReviewsAsync(int roomId)
        {
            return await _context.Reviews
                .Where(r => r.RoomId == roomId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task AddReviewAsync(int roomId, string userId, int rating, string comment)
        {
            var review = new Review
            {
                RoomId = roomId,
                UserId = userId,
                Rating = Math.Clamp(rating, 1, 5),
                Comment = comment.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }
    }
}
