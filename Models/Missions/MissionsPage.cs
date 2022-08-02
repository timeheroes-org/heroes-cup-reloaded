using HeroesCup.Data.Models;
using HeroesCup.Web.Models;
using Piranha.AttributeBuilder;
using Piranha.Models;

namespace HeroesCup.Models;

[PageType(Title = "Missions page", UseBlocks = false)]
[ContentTypeRoute(Title = "Missions", Route = "/missions")]
public class MissionsPage : Page<MissionsPage>, ISocialNetworkPost
{
    public MissionsPage()
    {
        Missions = new HashSet<MissionViewModel>();
        MissionIdeas = new HashSet<MissionIdeaViewModel>();
        MissionsPerLocation = new Dictionary<string, int>();
    }

    public IEnumerable<MissionViewModel> Missions { get; set; }

    public IEnumerable<MissionIdeaViewModel> MissionIdeas { get; set; }

    public IDictionary<string, int> MissionsPerLocation { get; set; }

    public int MissionsCount { get; set; }

    public string SelectedLocation { get; set; }

    public IEnumerable<StoryViewModel> Stories { get; set; }

    public bool IsLoadMoreMissionsRequest { get; set; }

    public SocialNetworksMetaData SocialNetworksMetaData { get; set; }
}