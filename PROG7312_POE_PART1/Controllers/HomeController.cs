using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROG7312_POE_PART1.Models;

namespace PROG7312_POE_PART1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Getting the feedback and displaying the feedback on the Index page
            var latest = ReportController.GetFeedbacks(4);
            return View(latest);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult BasicServices()
        {
            return View();
        }

        public IActionResult Events()
        {
            return View();
        }

        public IActionResult RequestStatus()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            // List of Team Members Displayed on the Contact Us Page 
            var teamMembers = new List<TeamMembers>
            {
                //Team Member 1 (Freepik,2025)
                new TeamMembers { Name = "MR. Luvuyo Magalela",Position = "Executive Director", Email="edee@mandelametro.gov.za",Image="teammember1.jpg"},
                
                //Team Member 2 (Freepik,2025)
                new TeamMembers { Name = "MR. Henry Williams",Position = "Deputy Director: Beaches & Resorts", Email="hwilliams@mandelametro.gov.za",Image="teammember2.jpg"},
                
                //Team Member 3 (Freepik,2025)
                new TeamMembers { Name = "MRS. Lesley Dunderdale",Position = "Director : Electricity & Energy", Email="ldunderdale@mandelametro.gov.za",Image="teammember3.jpg"},
                
               //Team Member 4 (Freepik,2025)
                new TeamMembers { Name = "MR. Luke Deliwe",Position = "Manager: Air & Noise Pollution", Email="bhumani@mandelametro.gov.za",Image="teammember4.jpg"},
                
                //Team Member 5 (Freepik,2025)
                new TeamMembers { Name = "MRS. Phumeza Marotya",Position = "Manager: Animal & Pest Control", Email="pmarotya@mandelametro.gov.za",Image="teammember5.jpg"},
                
                //Team Member 6 (Freepik,2025)
                new TeamMembers { Name = "MS. Rebecca Abioye",Position = "Emergency Medical Services (EMS) Co-ordinator", Email="rabioye@mandelametro.gov.za",Image="teammember6.jpg"}
            };
            // returning the list of teamMembers
            return View(teamMembers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
