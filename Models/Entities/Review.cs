using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }
        public virtual Room? Room { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Rating (1 to 5 Stars)")]
        public int Rating { get; set; } = 5;

        [Required]
        [MaxLength(600)]
        [Display(Name = "Guest Feedback")]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

