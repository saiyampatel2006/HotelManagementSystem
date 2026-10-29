using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagementSystem.Models.Entities
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Room Category")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(10, 50000)]
        [Display(Name = "Starting Price (per night)")]
        public decimal BasePrice { get; set; }

        [Required]
        [Range(1, 12)]
        [Display(Name = "Standard Guests")]
        public int DefaultCapacity { get; set; } = 2;

        [Required]
        [MaxLength(500)]
        [Display(Name = "Hero Image URL")]
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}

