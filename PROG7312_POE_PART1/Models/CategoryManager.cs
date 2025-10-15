namespace PROG7312_POE_PART1.Models
{
    public class CategoryManager
    {
        // The name of the entity that owns the categories
        public string Name { get; set; }

        // A HashSet to store unique categories (case-insensitive)
        public HashSet<string> Categories { get; set; }

        // A private static list of preset category names available to all instances
        private static readonly List<string> PresetCategories = new List<string>
        {
            "Environmental",
            "Market",
            "Education",
            "Food & Entertainment",
            "Arts & Culture",
            "Sports"
        };

        // Construcor : Initializing a new CategoryManager with a name and preset categories
        public CategoryManager(string name) 
        {
            Name = name;
            // Initialize the HashSet for storing categories, ignoring case sensitivity to avoid duplicates
            Categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            // Add the preset categories to the HashSet
            InitializePresetCategories();
        }

        // Adding the predefined categories to the Categories HashSet
        private void InitializePresetCategories()
        {
            foreach (var categoryName in PresetCategories)
            {
                // adding each preset category to the HashSet
                Categories.Add(categoryName);
            }
        }

        // Static method to expose preset category names for controllers or other classes
        public static List<string> GetPresetCategoryNames()
        {
            return PresetCategories;
        }

        // Adds a new category to the HashSet
        // Returns true if the category was added, false if it already exists
        public bool AddCategory(string category)
        {
            if (Categories.Add(category))
            {
                Console.WriteLine($"Category '{category}' was added to '{Name}'");
                return true;
            }
            else
            {
                Console.WriteLine($"Category '{category}' already exists for '{Name}'. No duplicates allowed");
                return false;
            }
        }

        // Removes a category from the HashSet
        // Returns true if removed successfully, false if the category does not exist
        public bool RemoveCategory(string category)
        {
            if (Categories.Remove(category))
            {
                Console.WriteLine($"Category '{category}' was removed from '{Name}'");
                return true;
            }
            else
            {
                Console.WriteLine($"Category '{category}' does not exist for '{Name}'");
                return false;
            }
        }

        // Displays all categories in the console for the current instance
        public void DisplayCategories()
        {
            Console.WriteLine($"\n--- Categories for {Name} ---");

            if (Categories.Count == 0)
            {
                Console.WriteLine("No categories assigned yet.");
                return;
            }

            //Print each category in the HashSet
            foreach (string category in Categories)
            {
                Console.WriteLine($"- {category}");
            }

            Console.WriteLine("------------------------------\n");
        }

        // Returns the categories as a List<string>
        public List<string> GetCategoriesList()
        {
            // This method returns the contents of the instance's HashSet
            return Categories.ToList();
        }

        // Checks if a given category exists in the HashSet
        public bool HasCategory(string category)
        {
            return Categories.Contains(category);
        }

        // Returns the total number of categories in the HashSet
        public int GetCategoryCount()
        {
            return Categories.Count;
        }
    }
}
