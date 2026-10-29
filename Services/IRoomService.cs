using HotelManagementSystem.Models.Entities;
using HotelManagementSystem.Models.ViewModels;

namespace HotelManagementSystem.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync(
            int? roomTypeId = null,
            int? guests = null,
            decimal? maxPrice = null,
            DateTime? checkIn = null,
            DateTime? checkOut = null,
            int? facilityId = null,
            string? sortBy = null);

        Task<Room?> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> GetFeaturedRoomsAsync();
        Task<IEnumerable<RoomType>> GetAllRoomTypesAsync();
        Task<IEnumerable<Facility>> GetAllFacilitiesAsync();
        Task<Room> CreateRoomAsync(RoomFormViewModel model);
        Task<Room?> UpdateRoomAsync(RoomFormViewModel model);
        Task<bool> DeleteRoomAsync(int id);
        Task<bool> UpdateRoomStatusAsync(int roomId, RoomStatus status);
        Task<bool> RoomNumberExistsAsync(string roomNumber, int? excludeId = null);
        Task<IEnumerable<Room>> GetSimilarRoomsAsync(int roomId, int roomTypeId, int take = 3);
        Task<IEnumerable<Review>> GetRoomReviewsAsync(int roomId);
        Task AddReviewAsync(int roomId, string userId, int rating, string comment);
    }
}

