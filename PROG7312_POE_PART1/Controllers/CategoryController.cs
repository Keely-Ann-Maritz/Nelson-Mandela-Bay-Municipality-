using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using PROG7312_POE_PART1.Models;
using System;
using System.Collections.Generic;

namespace PROG7312_POE_PART1.Controllers
{
    public class CategoryController : Controller
    {
        // Create a single instance of CategoryManager to handle category logic (add/remove/check)
        private static CategoryManager _categoryManager = new CategoryManager("Municipality Categories");

        // Public static property to expose the central manager ***
        public static CategoryManager SharedManager => _categoryManager;

        // Static list to hold all Category objects for displaying in the View
        private static List<Category> _categories = new List<Category>();

        // Counter to assign unique IDs to categories (used for identifying and deleting them)
        private static int _nextCategoryId = 1;

        // Helper method to prevent page caching(Hewlett, 2015)
        private void PreventPageCaching()
        {
            // Setting headers to prevent the browser from caching the page (Hewlett, 2015)
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }

        // Static contrustor - runs only once when the controller class is first loaded
        // It ensures that preset categories are initialized only once during the app lifetime
        static CategoryController()
        {
            _categories = new List<Category>();
            InitializePreset();
        }

        // Method to initialize default preset categories
        private static void InitializePreset()
        {
            // Retreiving the preset names from CategoryManager (Static helper method)
            var presetNames = CategoryManager.GetPresetCategoryNames();

            foreach (var name in presetNames)
            {
                // Create a new category object for each preset
                var category = new Category
                {
                    Id = _nextCategoryId++,
                    Name = name,
                    Description = $"Preset category for {name}",
                    CreatedDate = DateTime.Now
                };

                _categories.Add(category);
            }
        }

        // Displays the list of categories on the Index page 
        public IActionResult Index()
        {
            return View(_categories);
        }

        // Renders the form to add a new category
        [HttpGet]
        public IActionResult AddCategory()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            // Prevent Caching for this secure page
            PreventPageCaching();

            return View();
        }

        // Handles form submission to add a new category
        [HttpPost]
        public IActionResult AddCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                // Check if the category already exists (case-insensitive)
                if (_categoryManager.HasCategory(model.Name))
                {
                    ModelState.AddModelError("Name", "This category already exists.");
                    return View(model);
                }

                // Add to the CategoryManager HashSet (internal logic)
                bool added = _categoryManager.AddCategory(model.Name);

                if (added)
                {
                    // Assign new unique ID and timestamp before saving
                    model.Id = _nextCategoryId++;
                    model.CreatedDate = DateTime.Now;

                    // Add to local list for display in View
                    _categories.Add(model);

                    // Display success message via TempData (visible after redirect)
                    TempData["Status"] = "Success";
                    TempData["Message"] = $"Category '{model.Name}' has been added successfully!";
                     
                    // Redirect to category list page
                    return RedirectToAction("ListCategory");
                }
            }

            // If validation fails, redisplay form with errors
            return View(model);
        }

        // Displays the list of all categories
        // Add parameters and default values
        public IActionResult ListCategory(int page = 1, int pageSize = 5) 
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }

            // Prevent Caching for this secure page
            PreventPageCaching();

            // The source collection is already _categories, which is initialized and managed statically.
            var categories = _categories
            .OrderByDescending(c => c.CreatedDate)
            .ToList();

            var total = categories.Count;

            // Validate page and pageSize parameters (W3Schools,2025)
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;

            // Calculate total pages (W3Schools,2025)
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            // Ensure the requested page is not greater than the total pages (W3Schools,2025)
            if (page > totalPages && totalPages > 0)
            {
                page = totalPages;
            }
            // Handle case with no items (W3Schools,2025)
            else if (totalPages == 0)
            {
                page = 1; 
            }

            // Apply paging to the collection (W3Schools,2025)
            var pagedItems = categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass pagination data to the view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = total; 

            // Pass the paged items as the Model
            return View(pagedItems);
        }

        // Handles deleting a category by its ID
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            // Find category by ID in the local list
            var category = _categories.FirstOrDefault(c => c.Id == id);

            if (category != null)
            {
                //Remove from both CategoryManager and local list
                _categoryManager.RemoveCategory(category.Name);
                _categories.Remove(category);

                //Show success message
                TempData["Status"] = "Success";
                TempData["Message"] = $"Category '{category.Name}' has been deleted successfully!";
            }
            else
            {
                //Show error if ID not found
                TempData["Status"] = "Error";
                TempData["Message"] = "Category not found.";
            }

            //Redirect back to the list after deletion
            return RedirectToAction("ListCategory");
        }
    }
}
