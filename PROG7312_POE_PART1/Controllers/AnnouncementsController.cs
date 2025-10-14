using Microsoft.AspNetCore.Mvc;
using PROG7312_POE_PART1.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace PROG7312_POE_PART1.Controllers
{
    public class AnnouncementsController : Controller
    {
        // Static field to hold the single instance of EventSearchManager for announcements
        private static EventSearchManager _announcementsManager;

        // A static counter to generate unique IDs for new announcements
        // Start after the hardcoded announcements ID 1-16
        private static int _nextAnnouncementId = 17; 

        // Helper method to prevent page caching(Hewlett, 2015)
        private void PreventPageCaching()
        {
            // Set headers to prevent the browser from caching the page
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }

        // Constructor: initializes the announcements manager and loads hard coded announcements if not already done
        public AnnouncementsController()
        {
            if (_announcementsManager == null)
            {
                // Creating new manager including announcements
                _announcementsManager = new EventSearchManager(includeAnnouncements: true);
                InitializeAnnouncements(); //Load hardcoded announcements
            }
        }

        // Handles the Announcements page, with pagination and sorting
        public IActionResult Announcements(int page = 1, int pageSize = 12, string sortDir = "desc")
        {
            // Get all announcements from the manager
            // Filter only announcements
            var all = _announcementsManager.GetAllEvents()
                .Where(a => a.IsAnnouncement) 
                .ToList();

            // Count total announcements
            var total = all.Count;

            // Ensure valid page and pageSize
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 12;

            // Apply paging
            var paged = all
                .Skip((page - 1) * pageSize) 
                .Take(pageSize)
                .ToList();

            // Create the ViewModel for the view
            var vm = new EventListViewModel
            {
                Items = paged, 
                Page = page,
                PageSize = pageSize, 
                TotalCount = total, 
                SortDirection = string.IsNullOrWhiteSpace(sortDir) ? "desc" : sortDir.ToLower() 
            };

            // Pass ViewModel to the view
            return View(vm);
        }

        // Adding announcement
        [HttpGet]
        public IActionResult AddAnnouncements()
        {
            // Checking if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            // Preventing Caching for this secure page
            PreventPageCaching();

            return View(new EventItem());
        }

        // Add announcement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAnnouncements(EventItem eventItem)
        {
            eventItem.IsAnnouncement = true;
            eventItem.Categories = null;

            ModelState.Clear();
            TryValidateModel(eventItem);

            if (ModelState.ContainsKey("Category"))
            {
                ModelState.Remove("Category");
            }
            if (ModelState.ContainsKey("Categories"))
            {
                ModelState.Remove("Categories");
            }

            if (ModelState.IsValid)
            {
                eventItem.Id = _nextAnnouncementId++;
                _announcementsManager.AddEvent(eventItem);

                TempData["Status"] = "Success";
                TempData["Message"] = $"Announcement '{eventItem.Title}' added successfully with ID: {eventItem.Id}.";

                // Redirect to the administrative list
                return RedirectToAction("ListAnnouncements");
            }

            ViewBag.Status = "Error";
            ViewBag.Message = "There were errors with your submission. Please check the form.";

            return View(eventItem);
        }

        // Deleting announcement-handles announcement deletion from the ListAnnouncements view
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveAnnouncement(int id)
        {
            // The manager is generic and removes by ID
            bool removed = _announcementsManager.RemoveEvent(id);

            if (removed)
            {
                TempData["Status"] = "Success";
                TempData["Message"] = $"Announcement with ID {id} has been successfully deleted.";
            }
            else
            {
                TempData["Status"] = "Error";
                TempData["Message"] = $"Could not find or delete announcement with ID {id}.";
            }

            // Redirect back to the administrative list of announcements
            return RedirectToAction("ListAnnouncements");
        }

        // Displaying announcements - Administrative list of all announcements
        public IActionResult ListAnnouncements(int page = 1, int pageSize = 5) 
        {
            // Checking if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            //Preventing Caching for this secure page
            PreventPageCaching();

            var announcementsOnly = _announcementsManager.GetAllEvents()
                .Where(e => e.IsAnnouncement)
                .OrderBy(e => e.Date)
                .ToList();

            var total = announcementsOnly.Count;

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            // Apply paging to the collection
            var pagedItems = announcementsOnly
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination data to the view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            // Pass the paged items as the Model
            return View(pagedItems);
        }


        // Loads initial hard coded announcements into the manager
        private void InitializeAnnouncements()
        {
            // List of hard coded announcement items
            var items = new List<EventItem>
            {
                new EventItem { Id = 1, Title = "Water Interruption Notice", Date = DateTime.Today.AddDays(1).AddHours(7), Location = "Ward 1 & 2, Uitenhage", Description = "Planned maintenance: water supply disruption expected between 07:00 and 11:00.", IsAnnouncement = true },
                new EventItem { Id = 2, Title = "Road Closure Advisory", Date = DateTime.Today.AddDays(2).AddHours(6), Location = "Main Rd between Church St & Caledon St, Uitenhage", Description = "Pothole repairs: expect partial lane closures. Use alternate routes.", IsAnnouncement = true },
                new EventItem { Id = 3, Title = "Library Holiday Hours", Date = DateTime.Today.AddDays(1).AddHours(12), Location = "Uitenhage Library", Description = "Adjusted opening hours this weekend due to maintenance.", IsAnnouncement = true },
                new EventItem { Id = 4, Title = "Recycling Collection Delay", Date = DateTime.Today.AddDays(2).AddHours(8), Location = "All Wards in Uitenhage", Description = "Collections delayed by one day due to public holiday.", IsAnnouncement = true },
                new EventItem { Id = 5, Title = "Electricity Maintenance Notice", Date = DateTime.Today.AddDays(3).AddHours(22), Location = "Uitenhage Industrial Area", Description = "Possible brief outages between 22:00 and 01:00 for substation upgrades.", IsAnnouncement = true },
                new EventItem { Id = 6, Title = "Beach Safety Advisory", Date = DateTime.Today.AddDays(0).AddHours(14), Location = "Van Stadens River Park", Description = "Strong currents reported at river access points; swim safely.", IsAnnouncement = true },
                new EventItem { Id = 7, Title = "Roadworks Update", Date = DateTime.Today.AddDays(1).AddHours(6), Location = "Cecil Ave & Prince Alfred St, Uitenhage", Description = "Lane shift in effect; expect delays during peak hours.", IsAnnouncement = true },
                new EventItem { Id = 8, Title = "Boil Water Advisory Lifted", Date = DateTime.Today.AddDays(0).AddHours(9), Location = "Wards 3-5, Uitenhage", Description = "Recent advisory lifted after successful testing. Water is safe to drink.", IsAnnouncement = true },
                new EventItem { Id = 9, Title = "Community Hall Maintenance", Date = DateTime.Today.AddDays(3).AddHours(9), Location = "Uitenhage Community Hall", Description = "Minor repairs scheduled; some rooms unavailable.", IsAnnouncement = true },
                new EventItem { Id = 10, Title = "Public Transport Schedule Update", Date = DateTime.Today.AddDays(4).AddHours(6), Location = "Uitenhage Town", Description = "Updated minibus taxi and bus schedules effective next week.", IsAnnouncement = true },
                new EventItem { Id = 11, Title = "Park Fountain Restoration", Date = DateTime.Today.AddDays(5).AddHours(11), Location = "Uitenhage Central Park", Description = "Restoration works commencing; area partially cordoned off.", IsAnnouncement = true },
                new EventItem { Id = 12, Title = "Storm Warning Advisory", Date = DateTime.Today.AddDays(1).AddHours(5), Location = "Uitenhage Metro Area", Description = "Severe weather expected; secure loose items and avoid travel if possible.", IsAnnouncement = true },
                new EventItem { Id = 13, Title = "Clinic Vaccination Drive", Date = DateTime.Today.AddDays(2).AddHours(9), Location = "Uitenhage Civic Clinic", Description = "Extended hours this weekend for vaccinations.", IsAnnouncement = true },
                new EventItem { Id = 14, Title = "Waste Collection Route Change", Date = DateTime.Today.AddDays(6).AddHours(7), Location = "Ward 5, Uitenhage", Description = "New route timings; refer to updated schedule online.", IsAnnouncement = true },
                new EventItem { Id = 15, Title = "Temporary Library Closure", Date = DateTime.Today.AddDays(7).AddHours(10), Location = "Uitenhage North Branch Library", Description = "Closed for inventory; reopens Monday.", IsAnnouncement = true },
                new EventItem { Id = 16, Title = "Road Safety Campaign", Date = DateTime.Today.AddDays(8).AddHours(8), Location = "Uitenhage Townwide", Description = "Increased traffic enforcement; drive safely.", IsAnnouncement = true }
            };

            // Load all announcements into the manager
            _announcementsManager.LoadEvents(items);
        }
    }
}
