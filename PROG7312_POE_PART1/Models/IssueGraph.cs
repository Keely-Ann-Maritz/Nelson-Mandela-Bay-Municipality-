namespace PROG7312_POE_PART1.Models
{
    public class IssueGraph
    {
        private readonly Dictionary<int, ReportIssue> _idToIssue = new Dictionary<int, ReportIssue>();
        // Using a read only dictionary  (Terevinto, 2017)
        private readonly Dictionary<int, List<(int neighborId, int weight)>> _adjacency = new Dictionary<int, List<(int, int)>>();

        // Expose issues and adjacency list as read-only (Terevinto, 2017)
        public IReadOnlyDictionary<int, ReportIssue> Issues => _idToIssue;
        public IReadOnlyDictionary<int, List<(int neighborId, int weight)>> Adjacency => _adjacency;

        // Build graph from a collection of issues
        public void Build(IEnumerable<ReportIssue> issues)
        {
            _idToIssue.Clear();
            _adjacency.Clear();

            var list = issues.ToList();
            foreach (var issue in list)
            {
                _idToIssue[issue.ReportId] = issue;
                _adjacency[issue.ReportId] = new List<(int, int)>();
            }

            // Create weighted edges based on similarity (Ray,2025)
            // same location and same category
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    var a = list[i];
                    var b = list[j];
                    int? weight = null;
                    if (!string.IsNullOrWhiteSpace(a.Location) &&
                        string.Equals(a.Location, b.Location, StringComparison.OrdinalIgnoreCase))
                    {
                        weight = 1;
                    }
                    else if (!string.IsNullOrWhiteSpace(a.Category) &&
                             string.Equals(a.Category, b.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        weight = 2;
                    }

                    if (weight.HasValue)
                    {
                        // Add bidirectional edge (Ray,2025)
                        _adjacency[a.ReportId].Add((b.ReportId, weight.Value));
                        _adjacency[b.ReportId].Add((a.ReportId, weight.Value));
                    }
                }
            }
        }

        // BFS traversal starting from a node
        public List<int> BfsTraversal(int startId, int? limit = null)
        {
            if (!_adjacency.ContainsKey(startId)) return new List<int>();

            var visited = new HashSet<int>();
            var order = new List<int>();
            var queue = new Queue<int>();
            queue.Enqueue(startId);
            visited.Add(startId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                order.Add(current);
                if (limit.HasValue && order.Count >= limit.Value) break;

                foreach (var (neighborId, _) in _adjacency[current].OrderBy(e => e.weight))
                {
                    if (visited.Add(neighborId)) queue.Enqueue(neighborId);
                }
            }
            return order;
        }

        // DFS traversal starting from a node
        public List<int> DfsTraversal(int startId, int? limit = null)
        {
            if (!_adjacency.ContainsKey(startId)) return new List<int>();

            var visited = new HashSet<int>();
            var order = new List<int>();

            void Dfs(int node)
            {
                if (limit.HasValue && order.Count >= limit.Value) return;
                visited.Add(node);
                order.Add(node);

                foreach (var (neighborId, _) in _adjacency[node].OrderBy(e => e.weight))
                {
                    if (!visited.Contains(neighborId))
                    {
                        Dfs(neighborId);
                        if (limit.HasValue && order.Count >= limit.Value) return;
                    }
                }
            }

            Dfs(startId);
            return order;
        }

        // Produces a minimal set of edges for clustering using an algorithm (Ray,2025)
        // Handles disconnected components (spanning forest)
        public List<(int u, int v, int w)> MinimumSpanningTree()
        {
            var result = new List<(int, int, int)>();
            if (_adjacency.Count == 0) return result;

            var visited = new HashSet<int>();

            foreach (var startId in _adjacency.Keys)
            {
                if (visited.Contains(startId)) continue;

                visited.Add(startId);
                var edges = new List<(int weight, int u, int v)>();
                foreach (var (n, w) in _adjacency[startId])
                {
                    edges.Add((w, startId, n));
                }

                while (edges.Count > 0)
                {
                    var best = edges.OrderBy(e => e.weight).First();
                    edges.Remove(best);
                    if (visited.Contains(best.v)) continue;

                    visited.Add(best.v);
                    result.Add((best.u, best.v, best.weight));

                    foreach (var (n, w) in _adjacency[best.v])
                    {
                        if (!visited.Contains(n)) edges.Add((w, best.v, n));
                    }
                }
            }

            return result;
        }
    }
}