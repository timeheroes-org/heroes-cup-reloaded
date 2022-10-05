namespace HeroesCup.Web.Models
{
    public class SearchResponseModel
    {
        public SearchResponseModel()
        {
            Items = new List<SearchItem>();
        }
        public List<SearchItem> Items { get; set; }
    }

    public class SearchItem
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
        public SearchResultType Type { get; set; }
        public string Image { get; set; }
        public string Id { get; set; }
        public string Slug { get; set; }
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