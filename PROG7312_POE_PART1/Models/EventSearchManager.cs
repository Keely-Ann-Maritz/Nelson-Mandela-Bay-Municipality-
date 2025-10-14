using Newtonsoft.Json.Linq;

namespace PROG7312_POE_PART1.Models
{
    public class EventSearchManager
    {
        private readonly bool includeAnnouncements;

        // Dictionary to store events by category (Event/Announcement)
        private Dictionary<string, List<EventItem>> eventsByCategory;

        // Dictionary to store events by date (YYYY-MM-DD format)
        private Dictionary<string, List<EventItem>> eventsByDate;

        // Dictionary to store all events by ID for quick lookup
        private Dictionary<int, EventItem> eventsById;

        // Queue to manage upcoming events in FIFO order (soonest first)
        private Queue<EventItem> upcomingEventsQueue;

        public EventSearchManager(bool includeAnnouncements = false)
        {
            this.includeAnnouncements = includeAnnouncements;
            eventsByCategory = new Dictionary<string, List<EventItem>>();
            eventsByDate = new Dictionary<string, List<EventItem>>();
            eventsById = new Dictionary<int, EventItem>();
            upcomingEventsQueue = new Queue<EventItem>();
        }

        // Adds an event to all the dictionary indexes for efficient searching
        public void AddEvent(EventItem eventItem)
        {
            if (eventItem == null) return;

            // Add to events by ID
            if (!eventsById.ContainsKey(eventItem.Id))
            {
                eventsById.Add(eventItem.Id, eventItem);
            }
            else
            {
                // Update existing
                eventsById[eventItem.Id] = eventItem; 
            }

            //Add categories (supports multiple). If none specified, assign "General".
            var categories = (eventItem.Categories != null && eventItem.Categories.Any())
                ? eventItem.Categories
                : new List<string> { "General" };

            foreach (var raw in categories)
            {
                var category = (raw ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(category)) continue;

                if (!eventsByCategory.ContainsKey(category))
                {
                    eventsByCategory[category] = new List<EventItem>();
                }
                if (!eventsByCategory[category].Any(e => e.Id == eventItem.Id))
                {
                    eventsByCategory[category].Add(eventItem);
                }
            }

            // Add to date dictionary (using date only, not time)
            string dateKey = eventItem.Date.ToString("yyyy-MM-dd");
            if (!eventsByDate.ContainsKey(dateKey))
            {
                eventsByDate[dateKey] = new List<EventItem>();
            }
            if (!eventsByDate[dateKey].Any(e => e.Id == eventItem.Id))
            {
                eventsByDate[dateKey].Add(eventItem);
            }

            // Maintain upcoming events queue by rebalancing minimal scope if needed
            // For simplicity and correctness, rebuild the queue when the item falls in future
            if (eventItem.Date >= DateTime.Now)
            {
                RebuildUpcomingQueue();
            }
        }

        // Loads multiple events into the search manager
        public void LoadEvents(IEnumerable<EventItem> events)
        {
            ClearAll();
            foreach (var eventItem in events)
            {
                AddEvent(eventItem);
            }
            RebuildUpcomingQueue();
        }

        // Searches events by category (Event or Announcement)
        public List<EventItem> SearchByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return GetAllEvents();

            string categoryKey = category.Trim();
            if (eventsByCategory.TryGetValue(categoryKey, out List<EventItem>? events))
            {
                return events.OrderBy(e => e.Date).ThenBy(e => e.Title).ToList();
            }

            return new List<EventItem>();
        }

        // Searches events by specific date
        public List<EventItem> SearchByDate(DateTime date)
        {
            string dateKey = date.ToString("yyyy-MM-dd");
            if (eventsByDate.TryGetValue(dateKey, out List<EventItem>? events))
            {
                return events.OrderBy(e => e.Date).ThenBy(e => e.Title).ToList();
            }

            return new List<EventItem>();
        }

        // Searches events by date range
        public List<EventItem> SearchByDateRange(DateTime startDate, DateTime endDate)
        {
            var results = new List<EventItem>();
            var currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                string dateKey = currentDate.ToString("yyyy-MM-dd");
                if (eventsByDate.TryGetValue(dateKey, out List<EventItem>? events))
                {
                    results.AddRange(events);
                }
                currentDate = currentDate.AddDays(1);
            }

            return results.OrderBy(e => e.Date).ThenBy(e => e.Title).ToList();
        }

        // Searches events by title (case-insensitive partial match)
        public List<EventItem> SearchByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return GetAllEvents();

            string searchTerm = title.ToLower().Trim();
            var results = eventsById.Values
                .Where(e => e.Title.ToLower().Contains(searchTerm))
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Title)
                .ToList();

            return results;
        }

        // Combined search with multiple criteria
        public List<EventItem> Search(string category = null, DateTime? startDate = null,
            DateTime? endDate = null, string location = null, string title = null, string sortDir = "asc")
        {
            var allEvents = GetAllEvents();
            var results = allEvents.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim();
                results = results.Where(e => (e.Categories != null && e.Categories.Any())
                    ? e.Categories.Any(c => string.Equals(c?.Trim(), cat, StringComparison.OrdinalIgnoreCase))
                    : string.Equals("General", cat, StringComparison.OrdinalIgnoreCase));
            }

            if (startDate.HasValue)
            {
                results = results.Where(e => e.Date.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                results = results.Where(e => e.Date.Date <= endDate.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                string locationSearch = location.ToLower().Trim();
                results = results.Where(e => e.Location.ToLower().Contains(locationSearch));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                string titleSearch = title.ToLower().Trim();
                results = results.Where(e => e.Title.ToLower().Contains(titleSearch));
            }

            // Sorting
            bool descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            results = descending
                ? results.OrderByDescending(e => e.Date).ThenBy(e => e.Title)
                : results.OrderBy(e => e.Date).ThenBy(e => e.Title);

            return results.ToList();
        }

        // Gets all unique categories using Set
        public List<string> GetAvailableCategories()
        {
            return eventsByCategory.Keys.ToList();
        }

        // Gets all unique categories unsing Sets
        public List<string> GetAvailableLocations()
        {
            return eventsById.Values.Select(e => e.Location).Distinct().OrderBy(l => l).ToList();
        }

        // Gets all events
        public List<EventItem> GetAllEvents()
        {
            return eventsById.Values.OrderBy(e => e.Date).ThenBy(e => e.Title).ToList();
        }

        // Gets event by ID
        public EventItem? GetEventById(int id)
        {
            if (eventsById.TryGetValue(id, out EventItem? eventItem))
            {
                return eventItem;
            }
            return null;
        }

        // Removes an event from all dictionaries and Set
        public bool RemoveEvent(int id)
        {
            if (!eventsById.TryGetValue(id, out EventItem? eventItem))
            {
                return false;
            }

            eventsById.Remove(id);

            // Removing from all categories this event was assigned to
            var categoriesToRemove = (eventItem.Categories != null && eventItem.Categories.Any())
                ? eventItem.Categories
                : new List<string> { "General" };

            foreach (var raw in categoriesToRemove)
            {
                var category = (raw ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(category)) continue;
                if (eventsByCategory.ContainsKey(category))
                {
                    eventsByCategory[category].RemoveAll(e => e.Id == id);
                    if (eventsByCategory[category].Count == 0)
                    {
                        eventsByCategory.Remove(category);
                    }
                }
            }
            
            // Removing from the eventsByDate dictionary
            string dateKey = eventItem.Date.ToString("yyyy-MM-dd");
            if (eventsByDate.ContainsKey(dateKey))
            {
                eventsByDate[dateKey].RemoveAll(e => e.Id == id);
                if (eventsByDate[dateKey].Count == 0)
                {
                    eventsByDate.Remove(dateKey);
                }
            }

            // Rebuilding the upcoming queue to remove the deleted event
            RebuildUpcomingQueue();

            return true;
        }

        // Clears all data from the search manager
        public void ClearAll()
        {
            eventsByCategory.Clear();
            eventsByDate.Clear();
            eventsById.Clear();
            upcomingEventsQueue.Clear();
        }

        // Gets statistics about the events
        public void DisplayStatistics()
        {
            Console.WriteLine("Event Search Manager Statistics:");
            Console.WriteLine($"Total Events: {eventsById.Count}");
            Console.WriteLine($"Unique Dates: {eventsByDate.Count}");

            foreach (var category in eventsByCategory)
            {
                Console.WriteLine($"  {category.Key}: {category.Value.Count} events");
            }
        }

        // Rebuilds the upcoming events queue from current data, ordered soonest-first
        private void RebuildUpcomingQueue()
        {
            // Only future-dated items are queued. If the scheduled time has already passed (even earlier today),
            // it will not be enqueued and therefore cannot be dequeued via the "Dequeue Next" action.
            var orderedUpcoming = eventsById.Values
                .Where(e => e.Date >= DateTime.Now)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Title)
                .ToList();

            upcomingEventsQueue.Clear();
            foreach (var e in orderedUpcoming)
            {
                upcomingEventsQueue.Enqueue(e);
            }
        }

        // Peeks at the next upcoming event without removing it
        public EventItem? PeekNextUpcoming()
        {
            return upcomingEventsQueue.Count > 0 ? upcomingEventsQueue.Peek() : null;
        }

        // Dequeues the next upcoming event (FIFO)
        public EventItem? DequeueNextUpcoming()
        {
            if (upcomingEventsQueue.Count == 0) return null;
            // Dequeues only from the future-events queue. Past items are not present here.
            return upcomingEventsQueue.Dequeue();
        }

        // Returns the number of upcoming events currently queued
        public int GetUpcomingCount()
        {
            return upcomingEventsQueue.Count;
        }
    }
}

