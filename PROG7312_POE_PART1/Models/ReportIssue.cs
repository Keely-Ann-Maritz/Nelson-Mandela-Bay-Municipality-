using System.ComponentModel.DataAnnotations;

namespace PROG7312_POE_PART1.Models
{
    public class ReportIssue
    {
        // Gets and Sets 
        public int ReportId { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Media Attachment")]
        public string? MediaAttachment { get; set; }

        // Constructor
        public ReportIssue()
        {

        }

        // Constructor method
        public ReportIssue(int reportId, string location, string category, string description, string? mediaAttachment = null)
        {
            ReportId = reportId;
            Location = location;
            Category = category;
            Description = description;
            MediaAttachment = mediaAttachment;
        }
    }
}
