using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PROG7312_POE_PART1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PROG7312_POE_PART1.Controllers
{
    public class EventsController : Controller
    {
        // Static field to hold the single instance of EventSearchManager
        private static EventSearchManager _searchManager;
        private static EventRecommender _recommender;
        // Images for event cards (Métoule,2019)
        private readonly IWebHostEnvironment _webHostEnvironment;

        // A static counter to generate unique IDs for new events
        // Starting after the hardcoded events, which are ID numbers 1-16
        private static int _nextEventId = 17;

        // Helper method to prevent page caching 
        private void PreventPageCaching()
        {
            // Set headers to prevent the browser from caching the page (Hewlett, 2015)
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }

        //Constructor initializes the search manager and loads hard coded events if not already done
        public EventsController(IWebHostEnvironment webHostEnvironment)
        {
            // Images for Event cards (Métoule,2019)
            _webHostEnvironment = webHostEnvironment;

            if(_searchManager == null)
            {
                // Creating a new instance
                // Loading hardcoded events into the manager
                _searchManager = new EventSearchManager();
                InitializeEvents();
            }

            if(_recommender == null)
            {
                _recommender = new EventRecommender(_searchManager);
            }
        }

        // Events - handles the main events page, including a search bar and pagination
        public IActionResult Events(int page = 1, int pageSize = 12, string category = null,DateTime? startDate = null, DateTime? endDate = null, string location = null, string title = null,
            string sortDir = "asc")
        {
            // Perform search using the EventSearchManager, excluding announcements (Dot Net Tutorials,2025)
            var searchResults = _searchManager
                 .Search(category, startDate, endDate, location, title, sortDir)
                 .Where(e => !e.IsAnnouncement)
                 .ToList();

            // Priority Queue - keeping track of user search and updating the recommendations based on user search
            _recommender.RecordSearchAndQueue(category, title);
            List<EventItem> recommendations = new();

            // Only recommending recommendations after 3 searches
            if(_recommender.TotalSearches >= 3)
            {
                recommendations = _recommender.GetRecommendations();
            }

            // Passing the recommendations to the view using a viewbag
            ViewBag.RecommendedEvents = recommendations;

            //Count total events returned by search
            var total = searchResults.Count;

            //Ensure valid page and pageSize values (W3Schools, 2025)
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 12;

            //Select only the events for the current page (W3Schools, 2025)
            var pagedItems = searchResults
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Creating the viewmodel for the view 
            // Events for the current page, page number,number of items per page, total number of events, currently selected category,
            // start date filter, end date filter, title search filter, sorting 
            var vm = new EventListViewModel
            {
                Items = pagedItems,
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                SearchCategory = category ?? string.Empty,
                SearchStartDate = startDate,
                SearchEndDate = endDate,
                SearchTitle = title ?? string.Empty,
                SortDirection = string.IsNullOrWhiteSpace(sortDir) ? "asc" : sortDir.ToLower(),
                
                // List of available categories for filtering
                AvailableCategories = CategoryController.SharedManager.GetCategoriesList().OrderBy(c => c).ToList(),

                // Next upcoming events and total count of upcoming events
                NextUpcomingEvent = _searchManager.PeekNextUpcoming(),
                UpcomingCount = _searchManager.GetUpcomingCount()
            };

            // Passing the viewmodel to the view
            return View(vm);
        }

        // Handles the dequeue action for the next upcoming event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DequeueNext()
        {
            // Peek the next upcoming event 
            var next = _searchManager.PeekNextUpcoming();
            if (next == null)
            {
                // Informing the user if there are no events
                // Redirecting to the Events page
                TempData["Message"] = "No upcoming events to dequeue.";
                return RedirectToAction("Events");
            }

            // Dequeue the next upcoming event
            var dequeued = _searchManager.DequeueNextUpcoming();

            if (dequeued != null)
            {
                // Remove from the main collections so it no longer appears in lists
                _searchManager.RemoveEvent(dequeued.Id);
                TempData["Message"] = $"Dequeued next: {dequeued.Title} on {dequeued.Date:dd MMM yyyy HH:mm}.";
            }

            else
            {
                TempData["Message"] = "No upcoming events to dequeue.";
            }

            // Redirecting to the Events page
            return RedirectToAction("Events");
        }

        // Add Events 
        [HttpGet]
        public IActionResult AddEvents()
        {
            //Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            // Prevent chaching for this secure page
            PreventPageCaching();

            // Retreive the list of all available categories from the shared categoryManager
            // Sorting categories alphabetically 
            List<string> availableCategories = CategoryController.SharedManager
                .GetCategoriesList()
                .OrderBy(c => c) 
                .ToList();

            // Passing the list of categories to the view using a viewbag
            ViewBag.AvailableCategories = availableCategories;

            // Return the view with a new, empty EventItem model
            return View(new EventItem());
        }

        // Add Events
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEvents(EventItem eventItem, string category)
        {
            // The IsAnnouncement checkbox is hidden in the view, so we explicitly set it to false for events.
            eventItem.IsAnnouncement = false;

            // Ensure an image was uploaded
            if (eventItem.UploadedImage == null || eventItem.UploadedImage.Length == 0)
            {
                ModelState.AddModelError("UploadedImage", "Please upload an event image."); 
            }
            else
            {
                Console.WriteLine($"Uploaded image: {eventItem.UploadedImage.FileName}");

                // Getting root path (Raddevus,2024)
                string root = _webHostEnvironment.WebRootPath; 
                string tempFolder = Path.Combine(root, "Images", "Events", "Temp"); 

                // Adding an image folder if one does not exist
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder); 

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(eventItem.UploadedImage.FileName);
                string filePath = Path.Combine(tempFolder, fileName);

                // Save file to disk (Raddevus,2024)
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    eventItem.UploadedImage.CopyTo(stream);
                }

                // Store relative path for displaying in the view (Raddevus,2024)
                eventItem.ImagePath = "/Images/Events/Temp/" + fileName;
                Console.WriteLine($"Stored relative path: {eventItem.ImagePath}");
            }

            // Reset ModelState and re-validate
            ModelState.Clear();
            TryValidateModel(eventItem);

            // Assign category if provided
            if (!string.IsNullOrWhiteSpace(category))
            {
                eventItem.Categories = new List<string> { category };
            }

            // Ensure at least one category is selected
            if (eventItem.Categories == null || !eventItem.Categories.Any())
            {
                ModelState.AddModelError("Category", "Please select an event category.");
            }

            // If model is valid, save event
            if (ModelState.IsValid)
            {
                eventItem.Id = _nextEventId++; 
                _searchManager.AddEvent(eventItem); 

                TempData["Status"] = "Success";
                TempData["Message"] = $"Event '{eventItem.Title}' added successfully with ID: {eventItem.Id}.";

                return RedirectToAction("ListEvents"); 
            }

            // Repopulate categories for view in case of errors
            ViewBag.AvailableCategories = CategoryController.SharedManager.GetCategoriesList().OrderBy(c => c).ToList();
            ViewBag.Status = "Error";
            ViewBag.Message = "There were errors with your submission. Please check the form.";

            return View(eventItem); 
        }

        // Removing of an Event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveEvent(int id)
        {
            // Calling the EventManager RemoveEvent method
            bool removed = _searchManager.RemoveEvent(id);
            if (removed) 
            {
                TempData["Status"] = "Success";
                TempData["Message"] = $"Event with ID {id} has been deleted successfully!";
            }
            else
            {
                TempData["Status"] = "Error";
                TempData["Message"] = $"Could noe find or delete the event with the ID {id}.";
            }

            // Redirecting to the administrative list of events
            return RedirectToAction("ListEvents");
        }

        // Administrative list of all events (excluding announcements) (W3Schools, 2025)
        public IActionResult ListEvents(int page=1, int pageSize=5)
        {
            // Checking if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            // Preventing Caching for this secure page
            PreventPageCaching();

            // Retrieve and filter ALL events (excluding announcements) and sort
            var eventsOnly = _searchManager.GetAllEvents()
                .Where(e => !e.IsAnnouncement)
                .OrderBy(e => e.Date)
                .ToList();

            var total = eventsOnly.Count;

            // Validating the page and pageSize parameters (W3Schools,2025)
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;

            //Calculate total pages (W3Schools,2025)
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            //Apply paging to the collection (W3Schools,2025)
            var pagedItems = eventsOnly
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            //Pass pagination data to the view (W3Schools,2025)
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            //Pass the paged items as the Model
            return View(pagedItems);
        }

        //Loads initial hard coded events into the search manager
        private void InitializeEvents()
        {
            // Hard coded list of events with preset categories and data
            // Images (FreePik,2025)
            var items = new List<EventItem>
            {
                new EventItem { Id = 1, Title = "Community Clean-up Day", Date = DateTime.Today.AddDays(3).AddHours(9), Location = "Van Stadens River Park", Description = "Join neighbors to clean litter along the river and park pathways.", IsAnnouncement = false, Categories = new List<string> { "Environmental" },ImagePath = "/Images/Events/Community_Clean-up_Day.jpg" },
                new EventItem { Id = 2, Title = "Mangrove Cleanup Initiative", Date = DateTime.Today.AddDays(2).AddHours(8), Location = "Swartkops Estuary Walkway", Description = "Help remove litter and debris from the estuary to protect local wildlife.", IsAnnouncement = false, Categories = new List<string> { "Environmental" },ImagePath = "/Images/Events/Mangrove_Cleanup_Initiative.jpg" },
                new EventItem { Id = 3, Title = "Uitenhage Farmers Market", Date = DateTime.Today.AddDays(5).AddHours(8), Location = "Uitenhage Civic Hall", Description = "Fresh produce, crafts, and live music from local artists.", IsAnnouncement = false, Categories = new List<string> { "Market" }, ImagePath = "/Images/Events/Uitenhage_Farmers_Market.jpg" },
                new EventItem { Id = 4, Title = "Community Safety Meeting", Date = DateTime.Today.AddDays(4).AddHours(18), Location = "Uitenhage Town Hall, Room B", Description = "Discussion on neighborhood watch initiatives and safety tips.", IsAnnouncement = false, Categories = new List<string> { "Education" }, ImagePath = "/Images/Events/Community_Safety_Meeting.jpg"  },
                new EventItem { Id = 5, Title = "Tree Planting in Riverside Park", Date = DateTime.Today.AddDays(9).AddHours(10), Location = "Uitenhage Riverside Park", Description = "Volunteer to help plant indigenous trees in the park.", IsAnnouncement = false, Categories = new List<string> { "Environmental" }, ImagePath = "/Images/Events/Tree_Planting_in_Riverside_Park.jpg"  },
                new EventItem { Id = 6, Title = "Youth Sports Day", Date = DateTime.Today.AddDays(7).AddHours(9), Location = "Uitenhage Sports Complex", Description = "Free entry for under-18s; activities include soccer, netball, and athletics.", IsAnnouncement = false, Categories = new List<string> { "Sports" }, ImagePath = "/Images/Events/Youth_Sports_Day.jpg"  },
                new EventItem { Id = 7, Title = "Food Truck Friday", Date = DateTime.Today.AddDays(6).AddHours(17), Location = "Uitenhage Market Square", Description = "Local vendors with live music and family-friendly activities.", IsAnnouncement = false, Categories = new List<string> { "Food & Entertainment" }, ImagePath = "/Images/Events/Food_Truck_Friday.jpg"  },
                new EventItem { Id = 8, Title = "Art in the Park", Date = DateTime.Today.AddDays(8).AddHours(11), Location = "Uitenhage Botanical Gardens", Description = "Open-air gallery showcasing works from local artists.", IsAnnouncement = false, Categories = new List<string> { "Arts & Culture" }, ImagePath = "/Images/Events/Art_in_the_Park.jpg"  },
                new EventItem { Id = 9, Title = "Job Readiness Workshop", Date = DateTime.Today.AddDays(10).AddHours(9), Location = "Uitenhage Community Center", Description = "CV writing, interview practice, and career guidance. Booking required.", IsAnnouncement = false, Categories = new List<string> { "Education" }, ImagePath = "/Images/Events/Job_Readiness_Workshop.jpg"  },
                new EventItem { Id = 10, Title = "Community Health Fair", Date = DateTime.Today.AddDays(2).AddHours(10), Location = "Uitenhage Civic Hall", Description = "Free screenings and wellness info from local clinics.", IsAnnouncement = false, Categories = new List<string> { "Health & Wellness" }, ImagePath = "/Images/Events/Community_Health_Fair.jpg"  },
                new EventItem { Id = 11, Title = "Night Market", Date = DateTime.Today.AddDays(12).AddHours(17), Location = "Old Uitenhage Town Square", Description = "Street food, crafts, and live performances into the evening.", IsAnnouncement = false, Categories = new List<string> { "Market", "Food & Entertainment" }, ImagePath = "/Images/Events/Night_Market.jpg"  },
                new EventItem { Id = 12, Title = "Book Swap Sunday", Date = DateTime.Today.AddDays(13).AddHours(11), Location = "Uitenhage Library", Description = "Bring a book, take a book. Kids corner available.", IsAnnouncement = false, Categories = new List<string> { "Education" }, ImagePath = "/Images/Events/Book_Swap_Sunday.jpg"  },
                new EventItem { Id = 13, Title = "Local History Walk", Date = DateTime.Today.AddDays(4).AddHours(9), Location = "Uitenhage Museum Steps", Description = "Guided tour of historic landmarks in Uitenhage.", IsAnnouncement = false, Categories = new List<string> { "Arts & Culture", "Education" }, ImagePath = "/Images/Events/Local_History_Walk.jpg"  },
                new EventItem { Id = 14, Title = "Community Coding Club", Date = DateTime.Today.AddDays(6).AddHours(15), Location = "Uitenhage Tech Hub", Description = "Beginner-friendly coding meetup for all ages.", IsAnnouncement = false, Categories = new List<string> { "Education" }, ImagePath = "/Images/Events/Community_Coding_Club.jpg"  },
                new EventItem { Id = 15, Title = "Beach Volleyball Tournament", Date = DateTime.Today.AddDays(7).AddHours(14), Location = "Van Stadens River Park Beach Area", Description = "Sign up teams of four; prizes for winners.", IsAnnouncement = false, Categories = new List<string> { "Sports" }, ImagePath = "/Images/Events/Beach_Volleyball_Tournament.jpg"  },
                new EventItem { Id = 16, Title = "Garden Workshop", Date = DateTime.Today.AddDays(3).AddHours(10), Location = "Uitenhage Botanical Gardens", Description = "Learn indigenous planting and water-wise gardening.", IsAnnouncement = false, Categories = new List<string> { "Environmental", "Education" }, ImagePath = "/Images/Events/Garden_Workshop.jpg"  }
            };

            //Load all events into the search manager
            _searchManager.LoadEvents(items);
        }
    }
}