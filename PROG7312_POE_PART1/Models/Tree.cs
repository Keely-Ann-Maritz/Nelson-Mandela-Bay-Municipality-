namespace PROG7312_POE_PART1.Models
{
    public class Tree<T>
    {
        public TreeNode<T> Root { get; set; }

        // Prints the tree structure from a given node
        public void PrintTree(TreeNode<T> node, string indent = "", bool last = true)
        {
            if (node == null) return;
            Console.WriteLine($"{indent} +- {node.Data}");

            // Adjust indentation for child nodes
            indent += last ? "  " : "| ";

            for (int i = 0; i < node.Children.Count; i++)
            {
                PrintTree(node.Children[i], indent, i == node.Children.Count - 1);
            }
        }

        // Finds and returns a node by its value
        public TreeNode<T> FindNode(TreeNode<T> node, T value)
        {
            if (node == null) return null;

            if (node.Data.Equals(value))
                return node;

            foreach (var child in node.Children)
            {
                var result = FindNode(child, value);
                if (result != null) return result;
            }

            // Value not found in tree
            return null;
        }

        // Prints the tree and highlights a specific node
        public void PrintTree(TreeNode<T> node, TreeNode<T> highlightNode, string indent = "", bool last = true)
        {
            if (node == null) return;

            // Highlight the selected node
            if (node.Equals(highlightNode))
                Console.WriteLine($"{indent} +- [{node.Data}]");
            else
                Console.WriteLine($"{indent} +- {node.Data}");

            indent += last ? "  " : "| ";

            for (int i = 0; i < node.Children.Count; i++)
            {
                PrintTree(node.Children[i], highlightNode, indent, i == node.Children.Count - 1);
            }
        }

        // Performs Breadth-First Search (level order traversal)
        public List<T> BreadthFirstSearch()
        {
            if (Root == null)
                return new List<T>();

            var visited = new List<T>();
            var queue = new Queue<TreeNode<T>>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                visited.Add(currentNode.Data);

                // Add all children to the queue
                foreach (var child in currentNode.Children)
                {
                    queue.Enqueue(child);
                }
            }
            return visited;
        }

        // Helper for recursive DFS
        private void DFSHelper(TreeNode<T> node, List<T> visited)
        {
            if (node == null) return;

            visited.Add(node.Data);

            // Visit each child recursively
            foreach (var child in node.Children)
            {
                DFSHelper(child, visited);
            }
        }

        // Performs Depth-First Search (pre-order traversal)
        public List<T> DepthFirstSearch()
        {
            if (Root == null)
                return new List<T>();

            var visited = new List<T>();
            DFSHelper(Root, visited);
            return visited;
        }
    }
}
