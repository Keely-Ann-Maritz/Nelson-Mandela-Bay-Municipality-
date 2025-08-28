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

        public IActionResult ContactUs()
        {
            var teamMembers = new List<TeamMembers>
            {
                new TeamMembers { Name = "MR. Luvuyo Magalela",Position = "Executive Director", Email="edee@mandelametro.gov.za",Image=""},
                new TeamMembers { Name = "MS. Pakama Dyani",Position = "Deputy Director: Beaches & Resorts", Email="pdyani@mandelametro.gov.za",Image=""},
                new TeamMembers { Name = "MRS. Lesley Dunderdale",Position = "Director : Electricity & Energy", Email="ldunderdale@mandelametro.gov.za",Image=""},
                new TeamMembers { Name = "MRS. Buyiswa Deliwe",Position = "Manager: Air & Noise Pollution", Email="bhumani@mandelametro.gov.za",Image=""},
                new TeamMembers { Name = "MRS. Phumeza Marotya",Position = "Manager: Animal & Pest Control", Email="pmarotya@mandelametro.gov.za",Image=""},
                new TeamMembers { Name = "MS. Rosa Blaauw",Position = "Emergency Medical Services (EMS) Co-ordinator", Email="rblaauw@mandelametro.gov.za",Image=""}
            };
            return View(teamMembers);
        }

        //Search bar method
        //public async Task<IActionResult> Search(string query)
        //{
        ////Create a new instance of the 'Cldv6211PoeContext' context (Troelsen and Japikse,2021)
        //using (var context = new Cldv6211PoeContext())
        //{
        //    //Retrieve products from the database context and filtering the products by product name or category (Stackoverflow,2019)
        //    var products = await context.Products
        //        .Where(p => p.ProductName.Contains(query) || p.Category.Contains(query))
        //        .ToListAsync();

        //    //No results found, pass this information to the view (Stackoverflow,2019)
        //    if (!products.Any())
        //    {
        //        ViewBag.NoResults = true;
        //    }

        //    //Continue to use "MyWork" or specific search result view (Troelsen and Japikse,2021)
        //    return View("MyWork", products);
        //}
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
