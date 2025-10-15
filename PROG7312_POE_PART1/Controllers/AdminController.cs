using Microsoft.AspNetCore.Mvc;
using PROG7312_POE_PART1.Models;

namespace PROG7312_POE_PART1.Controllers
{
    public class AdminController : Controller
    {
        // Hardcoded admin credentials
        private const string adminUsername = "admin";
        private const string adminPassword = "admin123";
        private const string adminFullname = "Administrator";

        // Helper method to prevent page caching
        private void PreventPageCaching()
        {
            // Set headers to prevent the broswer from caching the page (Hewlett, 2015)
            Response.Headers["Cache-Control"] = "no-cache, no-store,must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
        }

        // Admin Portal Home page
        public IActionResult AdminHome()
        {
            // Checking if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                // If not logged in, redirect to login page
                return RedirectToAction("AdminLogin");
            }

            // Prevent Caching ONLY if the user is logged in (before returning the view)
            PreventPageCaching();

            return View();
        }

        // Admin Login
        [HttpGet]
        public IActionResult AdminLogin()
        {
            // Checking if user is logged in
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                // If not logged in, redirect to login page
                return RedirectToAction("ListCategory","Category");
            }

            return View();
        }

        // Admin Login 
        [HttpPost]
        public IActionResult AdminLogin(AppUser model)
        {
            if (ModelState.IsValid)
            {
                // Checking the hardcoded credentials
                if(model.Username == adminUsername && model.Password == adminPassword)
                {
                    // Setting the session variables
                    HttpContext.Session.SetString("Username", adminUsername);
                    HttpContext.Session.SetString("FullName", adminFullname);
                    // Returing to the Admin Home page
                    return RedirectToAction("ListCategory","Category");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(model);
        }

        // Logout 
        public IActionResult Logout()
        {
            // Clear the session data
            HttpContext.Session.Clear();
            // Prevent caching on the Logout action itself, which should force the browser to re-request pages from the server on back navigation.
            PreventPageCaching();
            // Redirecting to the Admin Login after being logged out
            return RedirectToAction("Index","Home");
        }
    }
}
