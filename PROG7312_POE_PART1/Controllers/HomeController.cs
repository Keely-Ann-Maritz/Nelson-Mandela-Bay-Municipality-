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
            return View();
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
                new TeamMembers { Name = "MR. Luvuyo Magalela",Position = "Executive Director", Email="edee@mandelametro.gov.za",Image="teammember1.jpg"},
                new TeamMembers { Name = "MR. Henry Williams",Position = "Deputy Director: Beaches & Resorts", Email="hwilliams@mandelametro.gov.za",Image="teammember2.jpg"},
                new TeamMembers { Name = "MRS. Lesley Dunderdale",Position = "Director : Electricity & Energy", Email="ldunderdale@mandelametro.gov.za",Image="teammember3.jpg"},
                new TeamMembers { Name = "MR. Luke Deliwe",Position = "Manager: Air & Noise Pollution", Email="bhumani@mandelametro.gov.za",Image="teammember4.jpg"},
                new TeamMembers { Name = "MRS. Phumeza Marotya",Position = "Manager: Animal & Pest Control", Email="pmarotya@mandelametro.gov.za",Image="teammember5.jpg"},
                new TeamMembers { Name = "MS. Rebecca Abioye",Position = "Emergency Medical Services (EMS) Co-ordinator", Email="rabioye@mandelametro.gov.za",Image="teammember6.jpg"}
            };
            return View(teamMembers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
