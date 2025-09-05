using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PROG7312_POE_PART1.Models;
using System.Text.RegularExpressions;

namespace PROG7312_POE_PART1.Controllers
{
    public class ReportController : Controller
    {
        // Object List
        private static readonly List<ReportIssue> issues = new List<ReportIssue>();

        public IActionResult Index()
        {
            return View();
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

                    var safeFileName = Path.GetFileName(mediaFile.FileName);
                    var category = string.IsNullOrWhiteSpace(model.Category) ? "Uncategorized" : model.Category;
                    var sanitizedCategory = Regex.Replace(category, "[^A-Za-z0-9_-]+", "_");
                    var uniqueFileName = $"{model.ReportId}_{sanitizedCategory}_{safeFileName}"; // Includes report id and category
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
                return RedirectToAction("SuccessfulMessage");
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