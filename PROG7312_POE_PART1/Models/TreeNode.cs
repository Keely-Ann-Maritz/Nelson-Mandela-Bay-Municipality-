namespace PROG7312_POE_PART1.Models
{
    public class TreeNode<T>
    {
        // The data stored in this node
        public T Data { get; set; }

        // Reference to the parent node (null if root)
        public TreeNode<T> Parent { get; set; }

        // List of this node's child nodes
        public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();
    }
}
