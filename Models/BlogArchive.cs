using HeroesCup.Models.Regions;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;

namespace HeroesCup.Models;

[PageType(Title = "Blog archive", UseBlocks = false, IsArchive = true)]
public class BlogArchive : Page<BlogArchive>
{
    /// <summary>
    ///     Gets/sets the archive hero.
    /// </summary>
    [Region(Display = RegionDisplayMode.Setting)]
    public HeroRegion Hero { get; set; }

    /// <summary>
    ///     Gets/sets the post archive.
    /// </summary>
    public PostArchive<DynamicPost> Archive { get; set; }
}