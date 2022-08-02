using HeroesCup.Data.Models;

namespace HeroesCup.Web.ClubsModule.Models;

public class StoryListModel
{
    public IEnumerable<StoryListItem> Stories { get; set; }
}

public class StoryListItem
{
    public Guid Id { get; set; }

    public string StartText { get; set; }

    public Mission Mission { get; set; }

    public bool IsPublished { get; set; }

    public string LastUpdateOn { get; set; }
}