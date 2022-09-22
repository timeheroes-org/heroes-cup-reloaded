using HeroesCup.Models;
using HeroesCup.Web.Common;
using HeroesCup.Web.Models.Missions;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.AspNetCore.Services;

namespace HeroesCup.Controllers;
public class MissionsController : Controller
{
    private const string MissionsPageCountKey = "missionsPageCount";
    private const string MissionIdeasPageCountKey = "missionIdeasPageCount";
    private const string StoriesPageCountKey = "storiesPageCount";
    private readonly IApi _api;
    private readonly IConfiguration _configuration;
    private readonly IModelLoader _loader;
    private readonly IMetaDataProvider _medaDataProvider;
    private readonly int _missionsCount;
    private readonly IMissionsService _missionsService;
    private readonly ISessionService _sessionService;
    private readonly IWebUtils _webUtils;

    /// <summary>
    ///     Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    public MissionsController(
        IApi api,
        IModelLoader loader,
        IMissionsService missionsService,
        ISessionService sessionService,
        IConfiguration configuration,
        IWebUtils webUtils,
        IMetaDataProvider medaDataProvider)
    {
        _api = api;
        _loader = loader;
        _missionsService = missionsService;
        _sessionService = sessionService;
        _configuration = configuration;
        int.TryParse(_configuration["MissionsCount"], out _missionsCount);
        _webUtils = webUtils;
        _medaDataProvider = medaDataProvider;
    }

    /// <summary>
    ///     Gets the missions archive with the given id.
    /// </summary>
    /// <param name="id">The unique page id</param>
    /// <param name="loadRequest">The optional load more missions</param>
    /// <param name="year">The optional year</param>
    /// <param name="month">The optional month</param>
    /// <param name="page">The optional page</param>
    /// <param name="category">The optional category</param>
    /// <param name="tag">The optional tag</param>
    /// <param name="draft">If a draft is requested</param>
    [Route("missions")]
    public async Task<IActionResult> MissionsArchive(Guid id, bool loadRequest, string selectedLocation,
        int? year = null, int? month = null, int? page = null,
        Guid? category = null, Guid? tag = null, bool draft = false)
    {
        var model = await _loader.GetPageAsync<MissionsPage>(id, HttpContext.User, draft);

        var missionsCurrentPageCount =
            _sessionService.GetCurrentPageCount(HttpContext, loadRequest, MissionsPageCountKey);
        var missionIdeasCurrentPageCount =
            _sessionService.GetCurrentPageCount(HttpContext, loadRequest, MissionIdeasPageCountKey);
        var storiesCurrentPageCount =
            _sessionService.GetCurrentPageCount(HttpContext, loadRequest, StoriesPageCountKey);

        if (selectedLocation != null)
        {
            var parsedLocation = _missionsService.ParseLocation(selectedLocation);
            model.SelectedLocation = parsedLocation;
            model.Missions = _missionsService.GetMissionViewModelsByLocation(parsedLocation);
        }
        else if (loadRequest)
        {
            model.IsLoadMoreMissionsRequest = true;
            model.Missions = _missionsService.GetMissionViewModels().ToList();
        }
        else
        {
            model.Missions = _missionsService.GetMissionViewModels().Take(missionsCurrentPageCount * _missionsCount);
        }

        model.MissionIdeas = _missionsService.GetMissionIdeaViewModels()
            .Take(missionIdeasCurrentPageCount * _missionsCount);
        model.Stories = _missionsService.GetAllPublishedStoryViewModels()
            .Take(storiesCurrentPageCount * _missionsCount);

        model.MissionsPerLocation = _missionsService.GetMissionsPerLocation();
        model.MissionsCount = _missionsService.GetAllMissionsCount();

        model.SocialNetworksMetaData = _medaDataProvider.getMetaData(HttpContext, model.Slug, model.Title);

        return View(model);
    }

    [Route("mission/{slug}")]
    public async Task<IActionResult> MissionPost(string slug, bool draft = false)
    {
        var mission = await _missionsService.GetMissionViewModelBySlugAsync(slug);
        if (mission == null) return NotFound();

        var currentUrlBase = _webUtils.GetUrlBase(HttpContext);
        var url = $"{currentUrlBase}/mission/{mission.Slug}";
        var imageUrl = $"{currentUrlBase}/img/{mission.ImageFilename}";
        var siteCulture = await _webUtils.GetCulture(_api);
        var dateFormat = _configuration["PostDateFormat"];
        var model = new MissionPost
        {
            Mission = mission,
            StartDateAsLocalString = mission.StartDate.ToString(dateFormat, siteCulture),
            EndDateAsLocalString = mission.EndDate.ToString(dateFormat, siteCulture),
            CurrentUrlBase = currentUrlBase,
            SiteCulture = siteCulture,
            Title = mission.Title,
            Slug = mission.Slug,
            Category = "mission",
            SocialNetworksMetaData =
                _medaDataProvider.getMetaData(HttpContext, mission.Title, mission.Title, url, imageUrl)
        };

        return View(model);
    }

    [Route("mission-idea/{slug}")]
    public async Task<IActionResult> MissionIdeaPost(string slug, bool draft = false)
    {
        var missionIdea = await _missionsService.GetMissionIdeaViewModelBySlugAsync(slug);
        if (missionIdea == null) return NotFound();

        var currentUrlBase = _webUtils.GetUrlBase(HttpContext);
        var url = $"{currentUrlBase}/mission-idea/{missionIdea.Slug}";
        var imageUrl = $"{currentUrlBase}/img/{missionIdea.ImageFilename}";
        var siteCulture = await _webUtils.GetCulture(_api);
        var dateFormat = _configuration["PostDateFormat"];
        var model = new MissionIdeaPost
        {
            MissionIdea = missionIdea,
            CurrentUrlBase = currentUrlBase,
            StartDateAsLocalString = missionIdea.StartDate.ToString(dateFormat, siteCulture),
            EndDateAsLocalString = missionIdea.EndDate.ToString(dateFormat, siteCulture),
            Title = missionIdea.MissionIdea.Title,
            Slug = missionIdea.MissionIdea.Slug,
            Category = "mission-idea",
            SocialNetworksMetaData = _medaDataProvider.getMetaData(HttpContext, missionIdea.MissionIdea.Title,
                missionIdea.MissionIdea.Title, url, imageUrl)
        };

        return View(model);
    }

    [Route("story/{slug}")]
    public async Task<IActionResult> StoryPost(string slug, bool draft = false)
    {
        var story = await _missionsService.GetStoryViewModelByMissionSlugAsync(slug);
        if (story == null) return NotFound();

        var currentUrlBase = _webUtils.GetUrlBase(HttpContext);
        var url = $"{currentUrlBase}/story/{story.Mission.Slug}";
        var imageUrl = $"{currentUrlBase}/img/{story.HeroImageFilename}";
        var siteCulture = await _webUtils.GetCulture(_api);
        var dateFormat = _configuration["PostDateFormat"];
        var model = new StoryPost
        {
            Story = story,
            StartDateAsLocalString = story.Mission.StartDate.ToString(dateFormat, siteCulture),
            EndDateAsLocalString = story.Mission.EndDate.ToString(dateFormat, siteCulture),
            CurrentUrlBase = currentUrlBase,
            SiteCulture = siteCulture,
            Title = story.Mission.Title,
            Slug = story.Mission.Slug,
            Category = "story",
            SocialNetworksMetaData =
                _medaDataProvider.getMetaData(HttpContext, story.Mission.Title, story.Mission.Title, url, imageUrl)
        };

        return View(model);
    }

    [Route("missions/load-missions")]
    public IActionResult LoadMissions(Guid id, bool loadRequest, int? year = null, int? month = null, int? page = null,
        Guid? category = null, Guid? tag = null, bool draft = false)
    {
        //int missionsCurrentPageCount = sessionService.GetCurrentPageCount(HttpContext, loadRequest, MissionsPageCountKey);
        var missions = _missionsService.GetMissionViewModels()
            .Skip(_missionsCount);

        var missionsWithBanner = new MissionsWithBannerViewModel
        {
            ShownMissionsCount = _missionsCount,
            Missions = missions,
            MissionsCountPerPage = _missionsCount
        };

        return PartialView("_MissionsListWithBanner", missionsWithBanner);
    }


    [Route("missions/load-missionideas")]
    public IActionResult LoadMissionIdeas(Guid id, bool loadRequest, int? year = null, int? month = null,
        int? page = null,
        Guid? category = null, Guid? tag = null, bool draft = false)
    {
        //int missionIdeasCurrentPageCount = sessionService.GetCurrentPageCount(HttpContext, loadRequest, MissionIdeasPageCountKey);
        var missionIdeas = _missionsService.GetMissionIdeaViewModels();
        //.Skip((int)missionIdeasCurrentPageCount * _missionsCount);

        return PartialView("_MissionIdeasList", missionIdeas);
    }

    [Route("missions/load-stories")]
    public IActionResult LoadStories(Guid id, bool loadRequest, int? year = null, int? month = null, int? page = null,
        Guid? category = null, Guid? tag = null, bool draft = false)
    {
        //int storiesCurrentPageCount = sessionService.GetCurrentPageCount(HttpContext, loadRequest, StoriesPageCountKey);
        var stories = _missionsService.GetAllPublishedStoryViewModels();
        //.Skip((int)storiesCurrentPageCount * _missionsCount);

        return PartialView("_StoriesList", stories);
    }
}