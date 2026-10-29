using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models.Entities
{
    public class Facility
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Facility Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        [Display(Name = "Icon CSS Class")]
        public string IconClass { get; set; } = "bi-check2-circle";

        [MaxLength(250)]
        public string? Description { get; set; }

        // Navigation
        public virtual ICollection<RoomFacility> RoomFacilities { get; set; } = new List<RoomFacility>();
    }
}

