using System;
using System.ComponentModel.DataAnnotations;

namespace PROG7312_POE_PART1.Models
{
    public class EventItem
    {
        // Gets and Sets 
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a title.")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please select a date.")]
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter description.")]
        public string Description { get; set; } = string.Empty;
        public bool IsAnnouncement { get; set; }
        [Required(ErrorMessage = "Please select a category.")]
        public List<string> Categories { get; set; } = new List<string>();
        public string ImagePath { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please upload an event image.")]
        public IFormFile? UploadedImage { get; set; }
        public string? ImagePreviewBase64 { get; set; }
    }
}
