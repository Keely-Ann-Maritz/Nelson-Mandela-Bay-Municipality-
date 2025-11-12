using Microsoft.AspNetCore.Mvc;
using PROG7312_POE_PART1.Models;

namespace PROG7312_POE_PART1.Controllers
{
    public class JobController : Controller
    {
        // In-memory job store (kept simple to match current app patterns)
        private static readonly List<JobCard> jobCards = new List<JobCard>();

        // Public helpers to access jobs elsewhere
        public static JobCard? GetJobByIssueId(int issueReportId)
        {
            return jobCards.FirstOrDefault(j => j.IssueReportId == issueReportId);
        }

        // Find any job that covers the given issue, either as the primary or as a related issue (Microsoft Ignite, 2025)
        public static JobCard? FindJobCoveringIssue(int issueReportId)
        {
            return jobCards.FirstOrDefault(j =>
                j.IssueReportId == issueReportId ||
                (j.RelatedIssueIds != null && j.RelatedIssueIds.Contains(issueReportId)));
        }

        // Assigned a job to a technician (Microsoft Ignite, 2025)
        public static bool HasActiveJobForAssignee(string assignee)
        {
            return jobCards.Any(j =>
                string.Equals(j.AssignedTo, assignee, StringComparison.OrdinalIgnoreCase) &&
                (j.Status == "Assigned" || j.Status == "In Progress"));
        }

        public static IEnumerable<JobCard> GetAllJobs()
        {
            return jobCards.ToList();
        }

        private static bool HasActiveJobForIssue(int issueReportId)
        {
            var job = FindJobCoveringIssue(issueReportId);
            return job != null && (job.Status == "Assigned" || job.Status == "In Progress");
        }

        // Displaying a list of jobs
        public IActionResult ListJobs()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            return View(jobCards.OrderByDescending(j => j.AssignedAt).ToList());
        }

        // Assigning jobs to employees (GET)
        [HttpGet]
        public IActionResult AssignJob(int? issueReportId = null)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            // Only show issues that are not resolved/closed (Sven, 2011)
            var availableIssues = ReportController.GetAllIssues()
                .Where(i =>
                    !string.Equals(i.Status, "Resolved", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(i.Status, "Closed", StringComparison.OrdinalIgnoreCase) &&
                    !HasActiveJobForIssue(i.ReportId))
                .OrderBy(i => i.ReportId)
                .ToList();

            ViewBag.Issues = availableIssues;

            var model = new JobCard();
            if (issueReportId.HasValue)
            {
                model.IssueReportId = issueReportId.Value;

                // Suggest related issues with same category and location (not resolved/closed) (Sven, 2011)
                var baseIssue = ReportController.GetIssueById(issueReportId.Value);
                if (baseIssue != null)
                {
                    var related = availableIssues
                        .Where(x => x.ReportId != baseIssue.ReportId
                            && string.Equals(x.Location ?? "", baseIssue.Location ?? "", StringComparison.OrdinalIgnoreCase)
                            && string.Equals(x.Category ?? "", baseIssue.Category ?? "", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(x => x.ReportId)
                        .ToList();
                    ViewBag.RelatedIssues = related;
                }
            }
            return View(model);
        }

        // Assigning Jobs to the employees (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignJob(JobCard model, bool includeRelated = false)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            // Basic validation (Sven, 2011)
            if (!ModelState.IsValid)
            {
                ViewBag.Issues = ReportController.GetAllIssues()
                    .Where(i =>
                        !string.Equals(i.Status, "Resolved", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(i.Status, "Closed", StringComparison.OrdinalIgnoreCase) &&
                        !HasActiveJobForIssue(i.ReportId))
                    .OrderBy(i => i.ReportId)
                    .ToList();

                // Checking related suggestions to ensure the checkbox visible after validation errors (Sven, 2011)
                if (model.IssueReportId > 0)
                {
                    var all = (List<ReportIssue>)ViewBag.Issues;
                    var baseIssue = ReportController.GetIssueById(model.IssueReportId);
                    if (baseIssue != null)
                    {
                        var related = all
                            .Where(x => x.ReportId != baseIssue.ReportId
                                && string.Equals(x.Location ?? "", baseIssue.Location ?? "", StringComparison.OrdinalIgnoreCase)
                                && string.Equals(x.Category ?? "", baseIssue.Category ?? "", StringComparison.OrdinalIgnoreCase))
                            .OrderBy(x => x.ReportId)
                            .ToList();
                        ViewBag.RelatedIssues = related;
                    }
                }
                return View(model);
            }

            // Preventing duplicate active job for the same person (Sven, 2011)
            if (HasActiveJobForAssignee(model.AssignedTo))
            {
                ModelState.AddModelError("", "This person already has an active job assigned.");
                ViewBag.Issues = ReportController.GetAllIssues()
                    .Where(i =>
                        !string.Equals(i.Status, "Resolved", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(i.Status, "Closed", StringComparison.OrdinalIgnoreCase) &&
                        !HasActiveJobForIssue(i.ReportId))
                    .OrderBy(i => i.ReportId)
                    .ToList();

                // Keeping related suggestions (Sven, 2011)
                if (model.IssueReportId > 0)
                {
                    var all = (List<ReportIssue>)ViewBag.Issues;
                    var baseIssue = ReportController.GetIssueById(model.IssueReportId);
                    if (baseIssue != null)
                    {
                        var related = all
                            .Where(x => x.ReportId != baseIssue.ReportId
                                && string.Equals(x.Location ?? "", baseIssue.Location ?? "", StringComparison.OrdinalIgnoreCase)
                                && string.Equals(x.Category ?? "", baseIssue.Category ?? "", StringComparison.OrdinalIgnoreCase))
                            .OrderBy(x => x.ReportId)
                            .ToList();
                        ViewBag.RelatedIssues = related;
                    }
                }
                return View(model);
            }

            // Preventing multiple active jobs for the same issue (Sven, 2011)
            var existingForIssue = GetJobByIssueId(model.IssueReportId);
            if (existingForIssue != null && (existingForIssue.Status == "Assigned" || existingForIssue.Status == "In Progress"))
            {
                ModelState.AddModelError("", "This issue already has an active job card.");
                ViewBag.Issues = ReportController.GetAllIssues()
                    .Where(i =>
                        !string.Equals(i.Status, "Resolved", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(i.Status, "Closed", StringComparison.OrdinalIgnoreCase) &&
                        !HasActiveJobForIssue(i.ReportId))
                    .OrderBy(i => i.ReportId)
                    .ToList();

                // Keeping related suggestions (Sven, 2011)
                if (model.IssueReportId > 0)
                {
                    var all = (List<ReportIssue>)ViewBag.Issues;
                    var baseIssue = ReportController.GetIssueById(model.IssueReportId);
                    if (baseIssue != null)
                    {
                        var related = all
                            .Where(x => x.ReportId != baseIssue.ReportId
                                && string.Equals(x.Location ?? "", baseIssue.Location ?? "", StringComparison.OrdinalIgnoreCase)
                                && string.Equals(x.Category ?? "", baseIssue.Category ?? "", StringComparison.OrdinalIgnoreCase))
                            .OrderBy(x => x.ReportId)
                            .ToList();
                        ViewBag.RelatedIssues = related;
                    }
                }
                return View(model);
            }

            // Creating a job card (Microsoft Ignite,2025)
            model.JobId = jobCards.Count == 0 ? 1 : jobCards.Max(j => j.JobId) + 1;
            model.AssignedAt = DateTime.UtcNow;
            model.Status = "Assigned";

            // Checking related issues for the same category & location (Sven, 2011)
            if (includeRelated)
            {
                var baseIssue = ReportController.GetIssueById(model.IssueReportId);
                if (baseIssue != null)
                {
                    var related = ReportController.GetAllIssues()
                        .Where(x =>
                            x.ReportId != baseIssue.ReportId &&
                            !string.Equals(x.Status, "Resolved", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(x.Status, "Closed", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(x.Location ?? "", baseIssue.Location ?? "", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(x.Category ?? "", baseIssue.Category ?? "", StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.ReportId)
                        .ToList();
                    model.RelatedIssueIds = related;
                }
            }

            jobCards.Add(model);

            // Syncing issue status to In Progress when it is assigned
            var issue = ReportController.GetIssueById(model.IssueReportId);
            if (issue != null)
            {
                issue.Status = "In Progress";
            }
            
            // Syncing related issues status if included
            if (model.RelatedIssueIds != null && model.RelatedIssueIds.Count > 0)
            {
                foreach (var rid in model.RelatedIssueIds)
                {
                    var rel = ReportController.GetIssueById(rid);
                    if (rel != null)
                    {
                        rel.Status = "In Progress";
                    }
                }
            }

            // Error and successful messages, informing the user the job was assigned (Tutorials Teacher,2024)
            TempData["AdminStatus"] = "Success";
            var countInfo = (model.RelatedIssueIds != null && model.RelatedIssueIds.Count > 0)
                ? $" and includes {model.RelatedIssueIds.Count} related request(s)"
                : "";
            TempData["AdminMessage"] = $"Job {model.JobId} assigned to {model.AssignedTo}{countInfo}.";
            return RedirectToAction("ListJobs");
        }

        // Update job status (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateJobStatus(int jobId, string status)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            var job = jobCards.FirstOrDefault(j => j.JobId == jobId);
            if (job == null)
            {
                TempData["AdminStatus"] = "Error";
                TempData["AdminMessage"] = "Job not found.";
                return RedirectToAction("ListJobs");
            }

            // The statuses options are displayed as the following for the admin to select (Tutorials Teacher,2024)
            var allowed = new[] { "Assigned", "In Progress", "Resolved", "Closed" };
            if (!allowed.Contains(status))
            {
                TempData["AdminStatus"] = "Error";
                TempData["AdminMessage"] = "Invalid status.";
                return RedirectToAction("ListJobs");
            }

            job.Status = status;

            // Syncing the issue status
            var issue = ReportController.GetIssueById(job.IssueReportId);
            if (issue != null)
            {
                issue.Status = status;
            }

            // Syncing the related issue statuses 
            if (job.RelatedIssueIds != null && job.RelatedIssueIds.Count > 0)
            {
                foreach (var rid in job.RelatedIssueIds)
                {
                    var rel = ReportController.GetIssueById(rid);
                    if (rel != null)
                    {
                        rel.Status = status;
                    }
                }
            }

            // Success messages, informing the admin the status was changed (Tutorials Teacher,2024)
            TempData["AdminStatus"] = "Success";
            TempData["AdminMessage"] = $"Job {job.JobId} status updated to {status}.";
            return RedirectToAction("ListJobs");
        }
    }
}



