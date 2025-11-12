namespace PROG7312_POE_PART1.Models
{
    public class RequestStatusViewModel
    {
        public List<ReportIssue> AllIssues { get; set; } = new List<ReportIssue>();

        // Heap outputs the prioritized issues
        public List<ReportIssue> TopPriorityIssues { get; set; } = new List<ReportIssue>();

        // Jobs linked to the issues by the ReportId
        public Dictionary<int, JobCard> JobByIssueId { get; set; } = new Dictionary<int, JobCard>();

        // Graph data for UI inspection (Terevinto,2017)
        public Dictionary<int, ReportIssue> GraphNodes { get; set; } = new Dictionary<int, ReportIssue>();
        public Dictionary<int, List<(int neighborId, int weight)>> GraphAdjacency { get; set; } = new Dictionary<int, List<(int, int)>>();

        // Traversal results around the searched issue
        public List<int> BfsOrder { get; set; } = new List<int>();
        public List<int> DfsOrder { get; set; } = new List<int>();

        // Minimum spanning tree result for grouping and optimizing the clustering display of the detailed rows (Ray, 2025)
        public List<MstEdgeDetail> MinimumSpanningTreeEdges { get; set; } = new List<MstEdgeDetail>();

        // Grouping the MST rows for a compact display
        public List<GroupedMstRow> GroupedMst { get; set; } = new List<GroupedMstRow>();

        // Echo of search
        public int? SearchId { get; set; }
        public ReportIssue? SearchResult { get; set; }
        public bool SearchFound { get; set; }

        // Explanations surfaced to the UI to demonstrate DS usage
        public string PriorityExplanation { get; set; } = string.Empty;
        public string TraversalExplanation { get; set; } = string.Empty;
        public string MstExplanation { get; set; } = string.Empty;
    }

    // Gets and sets for the Tree and Graph display (Ray, 2025)
    public class MstEdgeDetail
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public int Weight { get; set; }
        public string SharedAttributes { get; set; } = string.Empty;
        public string FromLocation { get; set; } = string.Empty;
        public string ToLocation { get; set; } = string.Empty;
        public string FromCategory { get; set; } = string.Empty;
        public string ToCategory { get; set; } = string.Empty;
    }

    // Gets and sets for the Tree and Graph display (Ray, 2025)
    public class GroupedMstRow
    {
        public int FromId { get; set; }
        public string FromLocation { get; set; } = string.Empty;
        public string FromCategory { get; set; } = string.Empty;
        public List<int> ToIds { get; set; } = new List<int>();
        public string SharedSummary { get; set; } = string.Empty;
        public string WeightSummary { get; set; } = string.Empty;
    }
}

