using System.Collections.Generic;

namespace PROG7312_POE_PART1.Models
{
    public class EventListViewModel
    {
        public IEnumerable<EventItem> Items { get; set; } = new List<EventItem>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => PageSize <= 0 ? 0 : (TotalCount + PageSize - 1) / PageSize;
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        // Search parameters
        public string SearchCategory { get; set; } = string.Empty;
        public DateTime? SearchStartDate { get; set; }
        public DateTime? SearchEndDate { get; set; }
        public string SearchTitle { get; set; } = string.Empty;

        // Sorting - ascending 
        public string SortDirection { get; set; } = "asc"; 

        // Available options for dropdowns
        public List<string> AvailableCategories { get; set; } = new List<string>();

        // Search results info
        public bool IsSearchActive => !string.IsNullOrWhiteSpace(SearchCategory) || SearchStartDate.HasValue || SearchEndDate.HasValue || !string.IsNullOrWhiteSpace(SearchTitle);

        // Queue-related: surface next upcoming and count for UI
        public EventItem? NextUpcomingEvent { get; set; }
        public int UpcomingCount { get; set; }

    }
}
