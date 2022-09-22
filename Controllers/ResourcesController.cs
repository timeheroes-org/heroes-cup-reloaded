using HeroesCup.Web.Common;
using HeroesCup.Web.Models.Blocks;
using HeroesCup.Web.Models.Resources;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.AspNetCore.Services;

namespace HeroesCup.Web.Controllers;
public class ResourcesController : Controller
{
    private readonly IApi _api;
    private readonly IConfiguration _configuration;
    private readonly IModelLoader _loader;
    private readonly IMetaDataProvider _metaDataProvider;
    private readonly IVideoThumbnailParser _videoThumbnailParser;
    private readonly IWebUtils _webUtils;

    public ResourcesController(IApi api,
        IModelLoader loader,
        IConfiguration configuration,
        IWebUtils webUtils,
        IVideoThumbnailParser videoThumbnailParser,
        IMetaDataProvider metaDataProvider)
    {
        _api = api;
        _loader = loader;
        _configuration = configuration;
        _webUtils = webUtils;
        _videoThumbnailParser = videoThumbnailParser;
        _metaDataProvider = metaDataProvider;
    }

    // <summary>
    /// Gets the resources blog archive with the given id.
    /// </summary>
    /// <param name="id">The unique page id</param>
    /// <param name="year">The optional year</param>
    /// <param name="month">The optional month</param>
    /// <param name="page">The optional page</param>
    /// <param name="category">The optional category</param>
    /// <param name="tag">The optional tag</param>
    /// <param name="draft">If a draft is requested</param>
    [Route("resources")]
    public async Task<IActionResult> ResourcesArchive(Guid id, int? year = null, int? month = null, int? page = null,
        Guid? category = null, Guid? tag = null, bool draft = false)
    {
        var model = await _loader.GetPageAsync<ResourcesArchive>(id, HttpContext.User, draft);
        model.Archive = await _api.Archives.GetByIdAsync<ResourcePost>(id, page, category, tag, year, month);
        model.SocialNetworksMetaData = _metaDataProvider.getMetaData(HttpContext, model.Slug, model.Title);

        return View(model);
    }

    /// <summary>
    ///     Gets the resource with the given id.
    /// </summary>
    /// <param name="id">The unique page id</param>
    /// <param name="draft">If a draft is requested</param>
    [Route("resource")]
    public async Task<IActionResult> ResourcePost(Guid id, bool draft = false)
    {
        var model = await _loader.GetPostAsync<ResourcePost>(id, HttpContext.User, draft);
        var pages = await _api.Pages.GetAllAsync();
        var currentUrlBase = _webUtils.GetUrlBase(HttpContext);
        model.CurrentUrlBase = currentUrlBase;
        model.SiteCulture = await _webUtils.GetCulture(_api);
        var resourcesArchive = pages.FirstOrDefault(p => p.TypeId == "ResourcesArchive");
        if (resourcesArchive != null)
        {
            var resourcesArchiveId = resourcesArchive.Id;
            var resourcesPosts = await _api.Posts.GetAllAsync<ResourcePost>(resourcesArchiveId);
            var othersCount = 0;
            int.TryParse(_configuration["ResourcesDetailsOthersCount"], out othersCount);
            model.OtherResources = resourcesPosts.Where(r => r.Id != model.Id).Take(othersCount).ToList();

            var image = model.Hero != null && model.Hero.PrimaryImage.HasValue
                ? $"{currentUrlBase}{model.Hero.PrimaryImage.Media.PublicUrl.TrimStart(new[] { '~' })}"
                : $"{currentUrlBase}/{_configuration["FacebookDefaultImageUrl"]}";
            var url = $"{currentUrlBase}/{model.Category.Title}/{model.Slug}";
            model.SocialNetworksMetaData =
                _metaDataProvider.getMetaData(HttpContext, model.Title, model.Title, url, image);

            if (model.Type.Value == ResourcePostType.VIDEO)
            {
                var firstEmbedVideoBlock =
                    model.Blocks.Where(b => b.Type == "HeroesCup.Web.Models.Blocks.EmbeddedVideoBlock").FirstOrDefault()
                        as EmbeddedVideoBlock;
                if (firstEmbedVideoBlock != null)
                {
                    var videoUrl = firstEmbedVideoBlock.Source;
                    model.VideoThumbnail = _videoThumbnailParser.ParseDefaultThumbnailUrl(videoUrl);
                    model.VideoUrl = videoUrl;
                    model.SocialNetworksMetaData.VideoUrl = videoUrl;
                    model.SocialNetworksMetaData.VideoType = _configuration["FacebookDefaultVideoType"];
                    model.SocialNetworksMetaData.Image = model.VideoThumbnail;
                }
            }
        }

        return View(model);
    }
}