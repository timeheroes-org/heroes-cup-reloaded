namespace HeroesCup.Web.Models
{
    public class SearchResponseModel
    {
        public List<SearchItem> Items => new List<SearchItem>();
    }

    public class SearchItem
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
        public SearchResultType Type { get; set; }
    }

    public enum SearchResultType
    {
        Mission,
        Story,
        Resource,
        Club,
        Event
    }
}