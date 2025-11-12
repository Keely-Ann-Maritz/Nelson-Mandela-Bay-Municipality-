using System;
using System.Collections.Generic;
using System.Linq;

namespace PROG7312_POE_PART1.Models
{
    public class ReportIssueSearchTree
    {
        // The main tree structure 
        private Tree<ReportIssue> _tree;
        // Search by ReportId
        private Dictionary<int, TreeNode<ReportIssue>> _idIndex;

        public ReportIssueSearchTree()
        {
            _tree = new Tree<ReportIssue>();
            _idIndex = new Dictionary<int, TreeNode<ReportIssue>>();
        }

        // Builds a hierarchical tree of issues grouped by category
        public void BuildTree(IEnumerable<ReportIssue> issues)
        {
            if (issues == null || !issues.Any())
                return;

            // Reset existing tree and index
            _tree = new Tree<ReportIssue>();
            _idIndex.Clear();

            // Create root node for the entire structure
            var rootData = new ReportIssue
            {
                ReportId = 0,
                Location = "Root",
                Category = "All Service Requests",
                Description = "Root node for all service requests",
                Status = "Root"
            };
            _tree.Root = new TreeNode<ReportIssue> { Data = rootData };

            // Group issues by category
            var issuesByCategory = issues.GroupBy(i => i.Category ?? "Uncategorized");

            // Build category nodes and add related issues
            foreach (var categoryGroup in issuesByCategory)
            {
                var categoryNode = new TreeNode<ReportIssue>
                {
                    Data = new ReportIssue
                    {
                        // Special ID for category nodes
                        ReportId = -1, 
                        Location = categoryGroup.Key,
                        Category = categoryGroup.Key,
                        Description = $"Category: {categoryGroup.Key}",
                        Status = "Category"
                    },
                    Parent = _tree.Root
                };

                _tree.Root.Children.Add(categoryNode);

                // Add issues under each category node
                foreach (var issue in categoryGroup)
                {
                    var issueNode = new TreeNode<ReportIssue>
                    {
                        Data = issue,
                        Parent = categoryNode
                    };

                    categoryNode.Children.Add(issueNode);

                    // Index issues for fast lookup
                    _idIndex[issue.ReportId] = issueNode;
                }
            }
        }

        // Searches for an issue by its ReportId
        public ReportIssue SearchById(int reportId)
        {
            // Try direct lookup first
            if (_idIndex.ContainsKey(reportId))
                return _idIndex[reportId].Data;

            // Fallback to tree traversal
            if (_tree.Root == null)
                return null;

            var foundNode = FindNodeByReportId(_tree.Root, reportId);
            return foundNode?.Data;
        }

        // Recursively finds a node by ReportId
        private TreeNode<ReportIssue> FindNodeByReportId(TreeNode<ReportIssue> node, int reportId)
        {
            if (node == null) return null;

            if (node.Data != null && node.Data.ReportId == reportId)
                return node;

            foreach (var child in node.Children)
            {
                var result = FindNodeByReportId(child, reportId);
                if (result != null) return result;
            }

            return null;
        }

        // Returns all issues using breadth-first traversal
        public List<ReportIssue> GetAllIssues()
        {
            if (_tree.Root == null)
                return new List<ReportIssue>();

            var allNodes = _tree.BreadthFirstSearch();
            // Exclude root and category nodes
            return allNodes.Where(issue => issue.ReportId > 0).ToList(); 
        }

        // Returns all issues using depth-first traversal
        public List<ReportIssue> GetAllIssuesDFS()
        {
            if (_tree.Root == null)
                return new List<ReportIssue>();

            var allNodes = _tree.DepthFirstSearch();
            // Exclude root/category nodes
            return allNodes.Where(issue => issue.ReportId > 0).ToList(); 
        }

        // Returns the current tree
        public Tree<ReportIssue> GetTree()
        {
            return _tree;
        }
    }
}
