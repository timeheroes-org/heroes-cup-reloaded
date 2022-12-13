using System.Globalization;
using System.Text.RegularExpressions;
using HeroesCup.Data.Models;
using HeroesCup.Web.Exceptions;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.Common;
using HeroesCup.Web.Common.Extensions;
using HeroesCup.Web.Data;
using HeroesCup.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroesCup.Web.Services;

public class MissionsService : IMissionsService
{
    private readonly IConfiguration _configuration;
    private readonly string _dateTimeFormat;
    private readonly HeroesCupDbContext _dbContext;
    private readonly IImagesService _imageService;
    private readonly IMissionContentsService _missionContentsService;
    private readonly IMissionIdeasService _missionIdeasService;
    private readonly ISchoolYearService _schoolYearService;

    public MissionsService(
        HeroesCupDbContext dbContext,
        IMissionIdeasService missionIdeasService,
        IMissionContentsService missionContentsService,
        IImagesService imageService,
        IConfiguration configuration, ISchoolYearService schoolYearService)
    {
        _dbContext = dbContext;
        _missionIdeasService = missionIdeasService;
        _missionContentsService = missionContentsService;
        _imageService = imageService;
        _configuration = configuration;
        _schoolYearService = schoolYearService;
        _dateTimeFormat = _configuration["DateТimeFormat"];
    }

    public IEnumerable<MissionIdeaViewModel> GetMissionIdeaViewModels()
    {
        var timeheroesMissions = _missionIdeasService.GetAllPublishedMissionIdeas();
        return timeheroesMissions.Select(mi => MapMissionIdeaToMissionIdeaViewModel(mi));
    }

    public IEnumerable<MissionViewModel> GetMissionViewModels()
    {
        var missions = this.GetAllPublishedMissions();
        return missions.Select(m => MapMissionToMissionViewModel(m, this.GetMissionImagesIds(m.Id)));
    }

    public async Task<IEnumerable<MissionViewModel>> GetPinnedMissionViewModels()
    {
        var pinnedMissions = await this.GetPinnedMissions();
        return pinnedMissions.Select(m => MapMissionToMissionViewModel(m, this.GetMissionImagesIds(m.Id)));
    }

    public int GetAllMissionsCount()
    {
        return this.GetAllPublishedMissions().Count();
    }

    public IDictionary<string, int> GetMissionsPerLocation()
    {
        return this.GetAllPublishedMissions()
            .GroupBy(m => m.Location)
            .ToDictionary(x => x.Key, x => x.Count());
    }

    public IEnumerable<MissionViewModel> GetMissionViewModelsByLocation(string location)
    {
        return this.GetAllPublishedMissions()
            .Where(m => m.Location.Contains(location) || location.Contains(m.Location))
            .Select(m => MapMissionToMissionViewModel(m, this.GetMissionImagesIds(m.Id)));
    }

    public IQueryable<StoryViewModel> GetAllPublishedStoryViewModels()
    {
        return _dbContext.Stories
            .Include(s => s.Mission)
            .ThenInclude(m => m.MissionImages)
            .ThenInclude(m=>m.Image)
            .Include(s => s.Mission)
            .ThenInclude(m => m.Club)
            .Include(s => s.StoryImages)
            .ThenInclude(i=>i.Image)
            .Where(s => s.IsPublished == true)
            .OrderByDescending(s => s.Mission.StartDate).Select(x=> MapStoryToStoryViewModel(x, _imageService, false));
    }

    public IQueryable<Story> GetAllPublishedStories()
    {
        return _dbContext.Stories
            .Include(s => s.Mission)
            .ThenInclude(m => m.MissionImages)
            .Include(s => s.Mission)
            .ThenInclude(m => m.Club)
            .Include(s => s.StoryImages)
            .Where(s => s.IsPublished == true)
            .OrderByDescending(s => s.Mission.StartDate);
    }
    public async Task<MissionViewModel> GetMissionViewModelBySlugAsync(string slug)
    {
        var result = await this.GetMissionEditModelBySlugAsync(slug);
        if (result == null) return null;

        var model = MapMissionEditModelToMissionViewModel(result);

        return model;
    }

    public async Task<StoryViewModel> GetStoryViewModelByMissionSlugAsync(string missionSlug)
    {
        var story = await _dbContext.Stories
            .Include(s => s.Mission)
            .ThenInclude(m => m.MissionImages)
            .ThenInclude(mi => mi.Image)
            .Include(s => s.Mission)
            .ThenInclude(m => m.Club)
            .Include(s => s.StoryImages)
            .ThenInclude(si => si.Image)
            .FirstOrDefaultAsync(s => s.Mission.Slug == missionSlug);
        if (story == null) return null;

        var model = MapStoryToStoryViewModel(story, _imageService, true);

        return model;
    }

    public async Task<MissionIdeaViewModel> GetMissionIdeaViewModelBySlugAsync(string slug)
    {
        var result = await _missionIdeasService.GetMissionIdeaEditModelBySlugAsync(slug);
        if (result == null) return null;

        var model = MapMissionIdeaEditModelToMissionIdeaViewModel(result);

        return model;
    }

    public string ParseLocation(string location)
    {
        if (location == null) return null;

        var parsedResult = Regex.Replace(location, @"(\(\d\))", "");
        return parsedResult.Trim();
    }

    public async Task<MissionListModel> GetMissionListModelAsync(Guid? ownerId)
    {
        var missions = new List<Mission>();
        missions = await _dbContext.Missions
            .Include(m => m.Club)
            .Include(m => m.HeroMissions)
            .ThenInclude(hm => hm.Hero)
            .ToListAsync();

        if (ownerId.HasValue) missions = missions.Where(m => m.Club.OwnerId == ownerId.Value).ToList();

        var model = new MissionListModel
        {
            Missions = missions
                .OrderBy(m => m.IsPublished)
                .ThenByDescending(m => m.UpdatedOn)
                .Select(m => new MissionListItem
                {
                    Id = m.Id,
                    Title = m.Title,
                    ClubId = m.ClubId,
                    ClubName = m.Club.Name,
                    HeroesCount = m.HeroMissions != null ? m.HeroMissions.Count(hm => hm.MissionId == m.Id) : 0,
                    IsPublished = m.IsPublished,
                    IsPinned = m.IsPinned,
                    LastUpdateOn = m.UpdatedOn.ToUniversalDateTime().ToLocalTime().ToString(_dateTimeFormat)
                })
        };

        return model;
    }

    public async Task<MissionEditModel> CreateMissionEditModelAsync(Guid? ownerId)
    {
        var clubs = new List<Club>();
        clubs = await _dbContext.Clubs.ToListAsync();

        if (ownerId.HasValue) clubs = clubs.Where(c => c.OwnerId == ownerId.Value).ToList();

        var newMission = new Mission();
        newMission.Content = new MissionContent();
        newMission.OwnerId = ownerId.HasValue ? ownerId.Value : Guid.Empty;

        var model = new MissionEditModel
        {
            Mission = newMission,
            Clubs = clubs,
            ClubId = clubs.Count > 0 && newMission.OwnerId != Guid.Empty ? clubs.FirstOrDefault().Id : Guid.Empty
        };

        return model;
    }

    public async Task<Guid> SaveMissionEditModelAsync(MissionEditModel model)
    {
        var mission = await _dbContext.Missions
            .Include(c => c.MissionImages)
            .ThenInclude(m => m.Image)
            .Include(m => m.Club)
            .Include(m => m.Content)
            .FirstOrDefaultAsync(m => m.Id == model.Mission.Id && m.OwnerId == model.Mission.OwnerId);

        var slug = model.Mission.Title.Trim().ToSlug();
        slug = slug.Unidecode();

        var missionWithSameTitle = await _dbContext.Missions
            .Where(m => (m.Title == model.Mission.Title || m.Slug == slug) && m.Id != model.Mission.Id)
            .FirstOrDefaultAsync();

        if (missionWithSameTitle != null) throw new ExistingItemException();

        if (mission == null)
        {
            mission = new Mission();
            mission.Id = model.Mission.Id != Guid.Empty ? model.Mission.Id : Guid.NewGuid();
            var club = await _dbContext.Clubs.FirstOrDefaultAsync(c => c.Id == model.Mission.Club.Id);
            mission.OwnerId = club.OwnerId;
            mission.CreatedOn = DateTime.Now.ToUnixMilliseconds();
            _dbContext.Missions.Add(mission);
        }

        mission.Title = model.Mission.Title.Trim();
        mission.Slug = slug;
        mission.Location = model.Mission.Location;
        if (model.Mission.Stars != 0) mission.Stars = model.Mission.Stars;

        var dateFormat = _configuration["DateFormat"];
        var startDate = DateTime.ParseExact(model.UploadedStartDate, dateFormat, CultureInfo.InvariantCulture);
        var endDate = DateTime.ParseExact(model.UploadedEndDate, dateFormat, CultureInfo.InvariantCulture);
        mission.StartDate = startDate.StartOfTheDay().ToUnixMilliseconds();
        mission.EndDate = endDate.EndOfTheDay().ToUnixMilliseconds();
        if (model.Mission.DurationInHours != 0) mission.DurationInHours = model.Mission.DurationInHours;
        mission.SchoolYear = _schoolYearService.CalculateSchoolYear(startDate);
        await _missionContentsService.SaveOrUpdateMissionContent(model.Mission.Content, mission);

        // set mission organizer
        if (model.Mission.Club.Id != Guid.Empty)
        {
            var newOrganizator = _dbContext.Clubs.FirstOrDefault(h => h.Id == model.Mission.Club.Id);
            mission.Club = newOrganizator;
        }

        // set mission image
        if (model.Image != null)
        {
            var image = _imageService.MapFormFileToImage(model.Image);
            await _imageService.CreateMissionImageAsync(image, mission);
        }

        mission.UpdatedOn = DateTime.Now.ToUnixMilliseconds();

        await _dbContext.SaveChangesAsync();
        return mission.Id;
    }

    public async Task<bool> PublishMissionEditModelAsync(Guid missionId)
    {
        var mission = await _dbContext.Missions.FirstOrDefaultAsync(m => m.Id == missionId);
        if (mission == null) return false;

        mission.IsPublished = true;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnPublishMissionEditModelAsync(Guid missionId)
    {
        var mission = await _dbContext.Missions.FirstOrDefaultAsync(m => m.Id == missionId);
        if (mission == null) return false;

        mission.IsPublished = false;
        mission.IsPinned = false;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<MissionEditModel> GetMissionEditModelByIdAsync(Guid id, Guid? ownerId)
    {
        Mission mission = null;
        mission = await _dbContext.Missions
            .Include(m => m.Club)
            .Include(m => m.Content)
            .Include(c => c.HeroMissions)
            .ThenInclude(m => m.Hero)
            .Include(c => c.MissionImages)
            .Include(m => m.Story)
            .ThenInclude(s => s.StoryImages)
            .FirstOrDefaultAsync(c => c.Id == id);

        return await MapMissionToMissionEditModel(mission);
    }

    public TimeSpan GetMissionDuration(long startDate, long endDate)
    {
        return endDate.ToUniversalDateTime() - startDate.ToUniversalDateTime();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var mission = _dbContext.Missions.FirstOrDefault(c => c.Id == id);
        if (mission == null) return false;

        _dbContext.Missions.Remove(mission);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Mission>> GetMissionsBySchoolYear(string schoolYear)
    {
        if (string.IsNullOrEmpty(schoolYear) || string.IsNullOrWhiteSpace(schoolYear)) return null;

        var result = await _dbContext.Missions
            .Where(m => m.IsPublished)
            .Include(c => c.Club)
            .Include(m => m.HeroMissions)
            .ThenInclude(hm => hm.Hero)
            .Where(m => m.SchoolYear == schoolYear)
            .Where(m => m.Stars != 0 && m.HeroMissions != null && m.HeroMissions.Count > 0)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();

        return result;
    }

    public IEnumerable<string> GetMissionSchoolYears()
    {
        var schoolYears = _dbContext.Missions
            .Where(m => m.IsPublished && m.Stars != 0 && m.HeroMissions != null && m.HeroMissions.Count > 0)
            .GroupBy(m => m.SchoolYear)
            .Select(sy => sy.Key);

        return schoolYears;
    }

    public IEnumerable<Mission> GetAllPublishedMissions()
    {
        var missions = _dbContext.Missions
            .Where(m => m.IsPublished == true)
            .Include(m => m.HeroMissions)
            .Include(m => m.Club)
            .Include(m => m.Story)
            .OrderByDescending(m => m.StartDate);

        return missions.ToList();
    }

    public async Task<List<Mission>> GetAllPublishedMissionsWithContentAndImages()
    {
        return await _dbContext.Missions
            .Where(m => m.IsPublished == true)
            .Include(m => m.Content)
            .Include(m => m.HeroMissions)
            .Include(m => m.Club)
            .Include(m=>m.MissionImages).ToListAsync();
    }

    public async Task<bool> PinMissionEditModelAsync(Guid id)
    {
        var mission = await _dbContext.Missions.FirstOrDefaultAsync(m => m.Id == id);
        if (mission == null) return false;

        mission.IsPinned = true;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnpinMissionEditModelAsync(Guid id)
    {
        var mission = await _dbContext.Missions.FirstOrDefaultAsync(m => m.Id == id);
        if (mission == null) return false;

        mission.IsPinned = false;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Mission>> GetPinnedMissions()
    {
        var missions = await _dbContext.Missions
            .Where(m => m.IsPinned == true && m.IsPublished == true)
            .Include(m => m.Club)
            .Include(m => m.MissionImages)
            .Include(m => m.Story)
            .ThenInclude(s => s.StoryImages)
            .ToListAsync();

        int countOfPinnedMissionsOnHomePage;
        int.TryParse(_configuration["PinnedMissionsOnHomePageCount"], out countOfPinnedMissionsOnHomePage);

        if (missions.Count() < countOfPinnedMissionsOnHomePage)
        {
            var countOfMissionsToAdd = countOfPinnedMissionsOnHomePage - missions.Count();
            var latestMissions = await _dbContext.Missions
                .Where(m => m.IsPublished)
                .OrderByDescending(m => m.StartDate)
                .Take(countOfMissionsToAdd)
                .Include(m => m.Club)
                .Include(m => m.MissionImages)
                .Include(m => m.Story)
                .ThenInclude(s => s.StoryImages)
                .ToListAsync();

            var selectedIds = missions.Select(x => x.Id).ToList();
            var missionsToAdd = latestMissions.Where(m => !selectedIds.Contains(m.Id));

            missions.AddRange(missionsToAdd);
        }

        return missions.Take(countOfPinnedMissionsOnHomePage);
    }

    public async Task SaveMissionDurationHours(Mission mission, int durationHours, bool commit)
    {
        mission.DurationInHours = durationHours;
        if (commit) await _dbContext.SaveChangesAsync();
    }

    public async Task SaveMissionHeroes(Mission mission, IEnumerable<Guid> heroesIds, bool commit = false)
    {
        // set missions's heroes
        if (heroesIds != null && heroesIds.Any())
        {
            await DeleteHeroMissions(mission);
            await AddHeroesToMission(mission, heroesIds);
        }
        else
        {
            await DeleteHeroMissions(mission);
        }

        if (commit) await _dbContext.SaveChangesAsync();
    }

    public async Task<MissionEditModel> GetMissionEditModelBySlugAsync(string slug)
    {
        Mission mission = null;
        mission = await _dbContext.Missions
            .Include(m => m.Club)
            .Include(m => m.Content)
            .Include(c => c.HeroMissions)
            .ThenInclude(m => m.Hero)
            .Include(c => c.MissionImages)
            .ThenInclude(mi => mi.Image)
            .Include(m => m.Story)
            .ThenInclude(s => s.StoryImages)
            .ThenInclude(si => si.Image)
            .FirstOrDefaultAsync(c => c.Slug == slug);

        return await MapMissionToMissionEditModel(mission);
    }

    public IEnumerable<Tuple<string, string>> GetMissionImagesIds(Guid missionId)
    {
        return _dbContext.MissionImages
            .Include(mi => mi.Image)
            .Where(m => m.MissionId == missionId)
            .Select(mi => new Tuple<string, string>(mi.ImageId.ToString(), mi.Image.Filename));
    }

    private MissionIdeaViewModel MapMissionIdeaToMissionIdeaViewModel(MissionIdea missionIdea)
    {
        if (missionIdea == null) return null;

        return new MissionIdeaViewModel
        {
            Id = missionIdea.Id,
            Slug = missionIdea.Slug,
            MissionIdea = missionIdea,
            ImageFilename = missionIdea.MissionIdeaImages != null && missionIdea.MissionIdeaImages.Any()
                ? missionIdea.MissionIdeaImages.FirstOrDefault()?.Image.Filename
                : null, 
            IsExpired = missionIdea.EndDate.IsExpired(),
            IsSeveralDays = IsSeveralDays(missionIdea.StartDate, missionIdea.EndDate),
            Organization = !string.IsNullOrEmpty(missionIdea.Organization)
                ? missionIdea.Organization
                : _configuration["DefaultOrganization"]
        };
    }

    private MissionIdeaViewModel MapMissionIdeaEditModelToMissionIdeaViewModel(MissionIdeaEditModel missionIdeEditModel)
    {
        if (missionIdeEditModel == null) return null;

        return new MissionIdeaViewModel
        {
            Id = missionIdeEditModel.MissionIdea.Id,
            Slug = missionIdeEditModel.MissionIdea.Slug,
            ImageFilename = missionIdeEditModel.ImageFilename,
            MissionIdea = missionIdeEditModel.MissionIdea,
            StartDate = missionIdeEditModel.MissionIdea.StartDate.ConvertToLocalDateTime(),
            EndDate = missionIdeEditModel.MissionIdea.EndDate.ConvertToLocalDateTime(),
            IsExpired = missionIdeEditModel.MissionIdea.EndDate.IsExpired(),
            IsSeveralDays = IsSeveralDays(missionIdeEditModel.MissionIdea.StartDate,
                missionIdeEditModel.MissionIdea.EndDate),
            Organization =
                missionIdeEditModel.MissionIdea.Organization != null &&
                missionIdeEditModel.MissionIdea.Organization != string.Empty
                    ? missionIdeEditModel.MissionIdea.Organization
                    : _configuration["DefaultOrganization"]
        };
    }

    private static StoryViewModel MapStoryToStoryViewModel(Story story, IImagesService imagesService, bool includeImages = false)
    {
        if (story == null) return null;

        var images = story.StoryImages.Select(s => s.Image.Filename);
        string heroImageFilename = null;

        heroImageFilename = story.StoryImages != null && story.StoryImages.Any()
            ? story.StoryImages.FirstOrDefault()?.Image.Filename
            : story.Mission.MissionImages.FirstOrDefault()?.Image.Filename;


        return new StoryViewModel
        {
            Id = story.Id,
            Images = images,
            Content = story.Content,
            ClubName = story.Mission.Club.Name,
            HeroImageFilename = heroImageFilename,
            Mission = new MissionViewModel
            {
                Id = story.Mission.Id,
                Title = story.Mission.Title,
                Slug = story.Mission.Slug,
                ClubName = story.Mission.Club.Name,
                PostClubName = GetPostClubName(story.Mission.Club),
                ClubLocation = story.Mission.Club.Location,
                IsExpired = story.Mission.EndDate.IsExpired(),
                IsSeveralDays = IsSeveralDays(story.Mission.StartDate, story.Mission.EndDate),
                ImageFilename = story.Mission.MissionImages.FirstOrDefault() != null
                    ? story.Mission.MissionImages.FirstOrDefault()?.Image.Filename
                    : null,
                ImageId = story.Mission.MissionImages != null && story.Mission.MissionImages.Any()
                    ? story.Mission.MissionImages.FirstOrDefault()?.ImageId.ToString()
                    : null,
                StartDate = story.Mission.StartDate.ConvertToLocalDateTime(),
                EndDate = story.Mission.EndDate.ConvertToLocalDateTime()
            }
        };
    }

    private MissionViewModel MapMissionEditModelToMissionViewModel(MissionEditModel missionEditModel)
    {
        if (missionEditModel == null) return null;

        return new MissionViewModel
        {
            Id = missionEditModel.Mission.Id,
            Title = missionEditModel.Mission.Title,
            Slug = missionEditModel.Mission.Slug,
            ImageFilename = missionEditModel.ImageFilename,
            ImageId = missionEditModel.ImageId,
            Content = missionEditModel.Mission.Content,
            ClubName = missionEditModel.Mission.Club.Name,
            PostClubName = GetPostClubName(missionEditModel.Mission.Club),
            ClubLocation = missionEditModel.Mission.Club.Location,
            StartDate = missionEditModel.Mission.StartDate.ConvertToLocalDateTime(),
            EndDate = missionEditModel.Mission.EndDate.ConvertToLocalDateTime(),
            IsExpired = missionEditModel.Mission.EndDate.IsExpired(),
            IsSeveralDays = IsSeveralDays(missionEditModel.Mission.StartDate, missionEditModel.Mission.EndDate),
            Story = missionEditModel.Mission.Story != null
                ? new StoryViewModel
                {
                    Content = missionEditModel.Mission.Story != null ? missionEditModel.Mission.Story.Content : null,
                    ClubName = missionEditModel.Mission.Club.Name,
                    Images =
                        missionEditModel.Mission.Story.StoryImages != null &&
                        missionEditModel.Mission.Story.StoryImages.Any()
                            ? missionEditModel.Mission.Story.StoryImages.Select(s => String.Concat(s.Image.Filename.ToString(),".",s.Image.Extension))
                            : null
                }
                : null
        };
    }

    private MissionViewModel MapMissionToMissionViewModel(Mission mission,
        IEnumerable<Tuple<string, string>> missionImages)
    {
        if (mission == null) return null;

        return new MissionViewModel
        {
            Id = mission.Id,
            Title = mission.Title,
            Slug = mission.Slug,
            ClubName = mission.Club.Name,
            PostClubName = GetPostClubName(mission.Club),
            ClubLocation = mission.Club.Location,
            ImageFilename = missionImages.FirstOrDefault().Item2,
            ImageId = missionImages.FirstOrDefault().Item1,
            StartDate = mission.StartDate.ConvertToLocalDateTime(),
            EndDate = mission.EndDate.ConvertToLocalDateTime(),
            IsExpired = mission.EndDate.IsExpired(),
            IsSeveralDays = IsSeveralDays(mission.StartDate, mission.EndDate),
            Story = mission.Story != null
                ? new StoryViewModel
                {
                    Content = mission.Story.Content,
                    ClubName = mission.Club.Name,
                    Images = mission.Story.StoryImages != null && mission.Story.StoryImages.Any()
                        ? mission.Story.StoryImages.Select(s => String.Concat(s.Image.Filename.ToString(),".",s.Image.Extension))
                        : null
                }
                : null
        };
    }

    private static bool IsSeveralDays(long startDate, long endDate)
    {
        return endDate.ConvertToLocalDateTime().Date != startDate.ConvertToLocalDateTime().Date;
    }

    private static string GetPostClubName(Club club)
    {
        var clubNumber = club.OrganizationNumber != null && club.OrganizationNumber != string.Empty
            ? $" {club.OrganizationNumber} "
            : string.Empty;
        var clubName = $"\"{club.Name}\", {clubNumber} {club.OrganizationType} \"{club.OrganizationName}\"";

        return clubName;
    }

    private async Task DeleteHeroMissions(Mission mission, bool commit = false)
    {
        var heroMissions = _dbContext.HeroMissions.Where(hm => hm.MissionId == mission.Id);
        foreach (var heroMission in heroMissions) _dbContext.HeroMissions.Remove(heroMission);

        if (commit) await _dbContext.SaveChangesAsync();
    }

    private async Task AddHeroesToMission(Mission mission, IEnumerable<Guid> heroesIds, bool commit = false)
    {
        var heroMissions = new List<HeroMission>();
        foreach (var heroId in heroesIds)
        {
            var hero = _dbContext.Heroes.FirstOrDefault(h => h.Id == heroId);
            heroMissions.Add(new HeroMission
            {
                Hero = hero,
                Mission = mission
            });
        }

        mission.HeroMissions = heroMissions;

        if (commit) await _dbContext.SaveChangesAsync();
    }

    private async Task<MissionEditModel> MapMissionToMissionEditModel(Mission mission)
    {
        if (mission == null) return null;

        if (mission.Content == null) mission.Content = new MissionContent();

        var model = await CreateMissionEditModelAsync(null);
        model.Mission = mission;
        model.ClubId = mission.Club != null && mission.Club.Id != Guid.Empty ? mission.Club.Id : Guid.Empty;

        if (mission.MissionImages != null && mission.MissionImages.Count > 0)
        {
            var missionImage = await _imageService.GetMissionImage(mission.Id);
            model.ImageId = missionImage.ImageId.ToString();
            model.ImageFilename = missionImage.Image.Filename;
        }

        var dateFormat = _configuration["DateFormat"];
        model.UploadedStartDate = mission.StartDate.ToUniversalDateTime().ToLocalTime().ToString(dateFormat);
        model.UploadedEndDate = mission.EndDate.ToUniversalDateTime().ToLocalTime().ToString(dateFormat);
        model.Duration = GetMissionDuration(mission.StartDate, mission.EndDate);

        return model;
    }
}