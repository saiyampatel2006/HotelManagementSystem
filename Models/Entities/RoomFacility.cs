using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models.Entities
{
    public class RoomFacility
    {
        public int RoomId { get; set; }
        public virtual Room Room { get; set; } = null!;

        public int FacilityId { get; set; }
        public virtual Facility Facility { get; set; } = null!;
    }
}

