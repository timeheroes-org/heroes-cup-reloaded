using System.Globalization;
using HeroesCup.Models.Regions;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace HeroesCup.Web.Models.Resources;

[PostType(Title = "Resource post", UsePrimaryImage = false, UseExcerpt = false)]
[ContentTypeRoute(Title = "Default", Route = "/resource")]
public class ResourcePost : Post<ResourcePost>, IHeroesCupPost, ISocialNetworkPost
{
    public ResourcePost()
    {
        OtherResources = new List<ResourcePost>();
    }

    /// <summary>
    ///     Gets/sets the post hero.
    /// </summary>
    [Region]
    public HeroRegion Hero { get; set; }

    [Region(Title = "Subtitle")] public StringField Subtitle { get; set; }

    [Region(Title = "Type of resource")] public SelectField<ResourcePostType> Type { get; set; }

    public IEnumerable<ResourcePost> OtherResources { get; set; }

    public string VideoThumbnail { get; set; }

    public string VideoUrl { get; set; }

    public string CurrentUrlBase { get; set; }

    public CultureInfo SiteCulture { get; set; }

    public SocialNetworksMetaData SocialNetworksMetaData { get; set; }
}

public enum ResourcePostType
{
    PDF,
    VIDEO,
    ARTICLE
}