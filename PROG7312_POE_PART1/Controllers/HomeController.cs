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

        [HttpGet]
        public IActionResult RequestStatus(int? searchId)
        {
            // Get all issues
            var allIssuesList = ReportController.GetAllIssues().ToList();

            // Search tree (already part of the project)
            var searchTree = new ReportIssueSearchTree();
            searchTree.BuildTree(allIssuesList);

            // Build a similarity graph (category/location)
            var graph = new IssueGraph();
            graph.Build(allIssuesList);

            // Build a min-heap to prioritize issues by status and age
            var pq = new PriorityQueue<ReportIssue>();
            foreach (var issue in allIssuesList)
            {
                var statusPriority = issue.Status switch
                {
                    "Submitted" => 0,
                    "In Progress" => 1,
                    "Resolved" => 2,
                    "Closed" => 3,
                    _ => 4
                };
                var ageDays = (int)Math.Min(30, (DateTime.UtcNow - issue.SubmittedDate).TotalDays);
                var priority = statusPriority * 100 - ageDays;
                pq.Enqueue(issue, priority);
            }
            var topPriority = new List<ReportIssue>();
            var take = Math.Min(5, pq.Count);
            for (int i = 0; i < take; i++)
            {
                topPriority.Add(pq.Dequeue());
            }

            // Traversals
            List<int> bfs = new List<int>();
            List<int> dfs = new List<int>();
            ReportIssue? found = null;
            bool searchFound = false;

            if (searchId.HasValue)
            {
                found = searchTree.SearchById(searchId.Value);
                if (found != null)
                {
                    searchFound = true;
                    bfs = graph.BfsTraversal(found.ReportId, limit: 10);
                    dfs = graph.DfsTraversal(found.ReportId, limit: 10);
                }
                else
                {
                    TempData["Status"] = "Error";
                    TempData["Message"] = $"No service request found with ID: {searchId.Value}";
                }
            }

            // MST over similarity graph to form clusters/groups
            var mstEdges = graph.MinimumSpanningTree();
            var mstDetails = mstEdges.Select(edge =>
            {
                var fromIssue = graph.Issues[edge.u];
                var toIssue = graph.Issues[edge.v];
                var sharedReasons = new List<string>();
                if (!string.IsNullOrWhiteSpace(fromIssue.Location) && !string.IsNullOrWhiteSpace(toIssue.Location) &&
                    string.Equals(fromIssue.Location, toIssue.Location, StringComparison.OrdinalIgnoreCase))
                {
                    sharedReasons.Add($"Location: {fromIssue.Location}");
                }
                if (!string.IsNullOrWhiteSpace(fromIssue.Category) && !string.IsNullOrWhiteSpace(toIssue.Category) &&
                    string.Equals(fromIssue.Category, toIssue.Category, StringComparison.OrdinalIgnoreCase))
                {
                    sharedReasons.Add($"Category: {fromIssue.Category}");
                }

                var sharedText = sharedReasons.Count > 0
                    ? string.Join(" | ", sharedReasons)
                    : "General similarity";

                return new MstEdgeDetail
                {
                    FromId = edge.u,
                    ToId = edge.v,
                    Weight = edge.w,
                    SharedAttributes = sharedText,
                    FromLocation = fromIssue.Location,
                    ToLocation = toIssue.Location,
                    FromCategory = fromIssue.Category,
                    ToCategory = toIssue.Category
                };
            }).ToList();

            // Grouped MST rows (compact display)
            var grouped = mstDetails
                .GroupBy(e => e.FromId)
                .Select(g =>
                {
                    var weights = g.Select(x => x.Weight).Distinct().ToList();
                    var summaries = g.Select(x => x.SharedAttributes).Distinct().ToList();
                    var first = g.First();
                    return new GroupedMstRow
                    {
                        FromId = g.Key,
                        FromCategory = first.FromCategory,
                        FromLocation = first.FromLocation,
                        ToIds = g.Select(x => x.ToId).OrderBy(x => x).ToList(),
                        WeightSummary = weights.Count == 1 ? weights[0].ToString() : "mixed",
                        SharedSummary = summaries.Count == 1 ? summaries[0] : "Mixed (see details)"
                    };
                })
                .OrderBy(r => r.FromId)
                .ToList();

            var vm = new RequestStatusViewModel
            {
                AllIssues = allIssuesList.OrderByDescending(i => i.SubmittedDate).ToList(),
                TopPriorityIssues = topPriority,
                JobByIssueId = allIssuesList.ToDictionary(
                    i => i.ReportId,
                    i => JobController.FindJobCoveringIssue(i.ReportId)
                ),
                GraphNodes = graph.Issues.ToDictionary(k => k.Key, v => v.Value),
                GraphAdjacency = graph.Adjacency.ToDictionary(k => k.Key, v => v.Value.ToList()),
                BfsOrder = bfs,
                DfsOrder = dfs,
                MinimumSpanningTreeEdges = mstDetails,
                GroupedMst = grouped,
                SearchId = searchId,
                SearchResult = found,
                SearchFound = searchFound,
                PriorityExplanation = "Requests are ranked with a min-heap: urgent statuses bubble to the top and older items gain extra priority for quick attention.",
                TraversalExplanation = searchFound
                    ? "Breadth-first search reveals the quickest related connections, while depth-first surfaces deeper links around the selected request."
                    : "Run a search to reveal BFS and DFS insights for a specific request.",
                MstExplanation = "The MST links similar requests with the minimum total connection cost, highlighting clusters we can service together."
            };

            return View(vm);
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
