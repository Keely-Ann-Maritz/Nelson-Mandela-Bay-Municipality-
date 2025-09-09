using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PROG7312_POE_PART1.Models;
using System.Text.RegularExpressions;

namespace PROG7312_POE_PART1.Controllers
{
    public class ReportController : Controller
    {
        // Object lists for the user engagement strategy and report issue
        private static readonly List<Feedback> feedbacks = new List<Feedback>();
        private static readonly List<ReportIssue> issues = new List<ReportIssue>();

        public IActionResult Index()
        {
            // Getting the feedback and displaying the feedback on the Index page
            var feedback = ReportController.GetFeedbacks(4);
            return View(feedback);
        }

        // Displays the latest feedback from the users
        public static IEnumerable<Feedback> GetFeedbacks(int count)
        {
            return feedbacks
                .OrderByDescending(f => f.FeedbackId)
                .Take(count)
                .ToList();
        }

        // Displaying the user feedback in memory
        [HttpGet]
        public IActionResult FeedbackView()
        {
            return View(feedbacks);
        }

        // Displaying the successful message of reporting the issue
        [HttpGet]
        public IActionResult SuccessfulMessage(int id)
        {
            var issue = issues.FirstOrDefault(i => i.ReportId == id);
            if (issue == null)
            {
                TempData["Status"] = "Error";
                TempData["Message"] = "The reported issue could not be found!";
                return RedirectToAction("ReportIssues", "Report");
            }
            ViewBag.ReportId = id;
            return View();
        }

        // Displaying the submitted feedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FeedbackSubmission(Feedback model)
        {
            // Error message will display on the Feedback form page, informing the user to select the stars
            if (!ModelState.IsValid)
            {
                ViewBag.ReportId = model.ReportId;
                TempData["Status"] = "Error";
                TempData["Message"] = "Please provide a rating between 1 and 5.";
                return View("SuccessfulMessage","Report");
            }
            // Adding a feedback with the current date and time 
            model.FeedbackId = feedbacks.Count == 0 ? 1 : feedbacks.Max(f => f.FeedbackId) + 1;
            model.CreatedAt = DateTime.UtcNow;
            feedbacks.Add(model);

            // Success message will display on the Index page, informing the user their feedback was submitted
            TempData["Status"] = "Success";
            TempData["Message"] = "Thank you for providing your feedback!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ReportIssues()
        {
            // Preloaded Categories
            ViewBag.Categories = new SelectList(new[] { "Animal Cruelty", "Animal & Pest Control", "Road Related", "Electicity Related", "Fraud/Theft", "Illegal Dumping", "Service Delivery Related", "Waste Related", "Water & Sewerage Related" });
            return View(new ReportIssue());
        }

        // Displaying all the reported issues in memory
        [HttpGet]
        public IActionResult ReportIssueView()
        {
            return View(issues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReportIssues(ReportIssue model, IFormFile mediaFile)
        {
            try
            {
                // Upload file requirement
                if (mediaFile == null || mediaFile.Length == 0)
                {
                    ModelState.AddModelError("MediaFile", "Media file is required.");
                }

                // If the required form fields are not filled, the error message will display informing the user they need ot fill in the fields which are highlighted
                if (!ModelState.IsValid)
                {
                    ViewBag.Status = "Error";
                    ViewBag.Message = "Please correct the highlighted errors.";
                    ViewBag.Categories = new SelectList(new[] { "Animal Cruelty", "Animal & Pest Control", "Road Related", "Electicity Related", "Fraud/Theft", "Illegal Dumping", "Service Delivery Related", "Waste Related", "Water & Sewerage Related" });
                    return View(model);
                }

                // Assigning a new ID to the report ID
                model.ReportId = issues.Count == 0 ? 1 : issues.Max(i => i.ReportId) + 1;

                // File upload option
                if (mediaFile != null && mediaFile.Length > 0)
                {
                    var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsRoot))
                    {
                        Directory.CreateDirectory(uploadsRoot);
                    }

                    // Renaming the uploaded file to the following 'referenceNumber_Category_FileName'
                    // The files are saved to a uploads folder, which is created
                    var safeFileName = Path.GetFileName(mediaFile.FileName);
                    var category = string.IsNullOrWhiteSpace(model.Category) ? "Uncategorized" : model.Category;
                    var sanitizedCategory = Regex.Replace(category, "[^A-Za-z0-9_-]+", "_");
                    var uniqueFileName = $"{model.ReportId}_{sanitizedCategory}_{safeFileName}"; 
                    var filePath = Path.Combine(uploadsRoot, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        mediaFile.CopyTo(stream);
                    }
                    
                    model.MediaAttachment = $"/uploads/{uniqueFileName}";
                }

                issues.Add(model);

                // Displaying a message once
                TempData["Status"] = "Success";
                TempData["Message"] = "Issue reported successfully.";

                // Redirecting the user to the SuccessfulMessage after the Reporting of an issue form has successfully been submitted
                return RedirectToAction("SuccessfulMessage",new {id = model.ReportId});
            }
            catch (Exception ex)
            {
                // Error message
                ViewBag.Status = "Error";
                ViewBag.Message = $"An unexpected error occurred: {ex.Message}";
                ViewBag.Categories = new SelectList(new[] { "Animal Cruelty", "Animal & Pest Control", "Road Related", "Electicity Related", "Fraud/Theft", "Illegal Dumping", "Service Delivery Related", "Waste Related", "Water & Sewerage Related" });
                return View(model);
            }
        }
    }
}