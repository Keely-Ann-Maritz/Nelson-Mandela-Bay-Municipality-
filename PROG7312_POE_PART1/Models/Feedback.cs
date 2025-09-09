using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG7312_POE_PART1.Models
{
    public class Feedback
    {
        // Gets and Sets 
        public int FeedbackId { get; set; }

        [Required]
        [Range(1,5,ErrorMessage = "Please select a rating between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        public int ReportId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
