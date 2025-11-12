using System.ComponentModel.DataAnnotations;

namespace PROG7312_POE_PART1.Models
{
    public class JobCard
    {
        public int JobId { get; set; }

        // Linking to the reported issue
        [Required]
        public int IssueReportId { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; } = string.Empty;

        // String priority -  Low, Medium, High, Urgent
        [Required]
        public string Priority { get; set; } = "Medium";

        // Timestamp (Microsoft Ignite,2025)
        [Display(Name = "Assigned At")]
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        // Assigned, In Progress, Resolved, Closed
        [Required]
        public string Status { get; set; } = "Assigned";

        // Other issues covered by the same job, same location and category
        public List<int> RelatedIssueIds { get; set; } = new List<int>();
    }
}


