namespace PROG7312_POE_PART1.Models
{
    public class SearchTracker
    {
        private Dictionary<string, int> searches = new(StringComparer.OrdinalIgnoreCase);

        // Recording the users search 
        public void RecordSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return;
            }

            if (searches.ContainsKey(term))
            {
                searches[term]++;
            }
            else
            {
                searches[term] = 1;
            }
        }

        // Keeping track of what the user searches for 
        public int GetScore(string term)
        {
            if (!searches.ContainsKey(term)) 
            return 100;
            int score = 100 - (searches[term] - 1) * 10;
            return Math.Max(score, 10);
        }

        // Search count 
        public int TotalSearchCount => searches.Values.Sum();
    }
}
