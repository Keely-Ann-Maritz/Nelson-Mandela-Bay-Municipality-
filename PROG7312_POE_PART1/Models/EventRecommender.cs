namespace PROG7312_POE_PART1.Models
{
    public class EventRecommender
    {
        private EventSearchManager manager;
        private SearchTracker tracker = new();
        private EventQueue queue = new();
        private EventQueueNode queueHead;

        public EventRecommender(EventSearchManager manager)
        {
            this.manager = manager;
        }

        // Tracking the number of searches
        public int TotalSearches => tracker.TotalSearchCount;

        // Recording the Search History of the user and keeping track
        public void RecordSearchAndQueue(string category = null, string title = null)
        {
            // Recording the search hsitory of the user and keeping track of what the user searches (Microsoft, 2025)
            if (!string.IsNullOrWhiteSpace(category)) tracker.RecordSearch(category);
            if (!string.IsNullOrWhiteSpace(title)) tracker.RecordSearch(title);

            // clearing the previous recommendations before providing new recommendations
            queueHead = null;

            foreach (var e in manager.GetAllEvents())
            {
                int score = 0;

                // Matching the categories (Microsoft, 2025)
                if (!string.IsNullOrWhiteSpace(category) && e.Categories.Any(c => c.Equals(category, StringComparison.OrdinalIgnoreCase)))
                {
                    score += tracker.GetScore(category);
                }

                // Matching the title
                if (!string.IsNullOrWhiteSpace(title) && (e.Title.Contains(title, StringComparison.OrdinalIgnoreCase) || e.Description.Contains(title, StringComparison.OrdinalIgnoreCase) || 
                    e.Location.Contains(title, StringComparison.OrdinalIgnoreCase)))
                {
                    score += tracker.GetScore(title);
                }

                // Only queue valid events
                if (score > 0)
                {
                    queueHead = queue.Push(queueHead, e.Id, score);
                }

               
            }
        }

        // Recommending an event
        public EventItem? PopTopRecommendation()
        {
            if (queue.IsEmpty(queueHead)) return null;

            int eventId = queue.Peek(queueHead);
            queueHead = queue.Pop(queueHead);

            return manager.GetEventById(eventId);
        }

        // Getting Recommendations 
        public List<EventItem> GetRecommendations()
        {
            var recommendations = new List<EventItem>();
            var current = queueHead;

            while (current != null)
            {
                var eventItem = manager.GetEventById(current.EventId);
                if(eventItem != null)
                {
                    recommendations.Add(eventItem);
                }
                current = current.Next; 
            }
            return recommendations;
        }
    }
}
