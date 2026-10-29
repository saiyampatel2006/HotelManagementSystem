using HotelManagementSystem.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models.ViewModels
{
    public class CategoryRevenueItem
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalNightsBooked { get; set; }
        public int BookingCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public double PercentageOfRevenue { get; set; }
    }

    public class ReportsViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(30);

        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CompletedStays { get; set; }
        public int CancelledBookings { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal TaxCollected { get; set; }
        public decimal NetRevenue { get; set; }

        public List<CategoryRevenueItem> CategoryBreakdown { get; set; } = new();
        public IEnumerable<Booking> ReportBookings { get; set; } = new List<Booking>();
    }
}

