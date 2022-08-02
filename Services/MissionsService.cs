using HeroesCup.Web.Common;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Data.Models;
using HeroesCup.Web.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using HeroesCup.Web.ClubsModule.Exceptions;
using HeroesCup.Web.Common.Extensions;
using HeroesCup.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroesCup.Web.Services
{
    public class MissionsService : IMissionsService
    {
        private readonly HeroesCupDbContext _dbContext;
        private readonly IMissionsService _missionsService;
        private readonly IMissionIdeasService _missionIdeasService;
        private readonly IMissionContentsService _missionContentsService;
        private readonly IStoriesService _storiesService;
        private readonly ISchoolYearService _schoolYearService;
        private readonly IImagesService _imageService;
        private readonly IConfiguration _configuration;
        private readonly string _dateTimeFormat;

        public MissionsService(
            HeroesCupDbContext dbContext,
            IMissionsService missionsService,
            IMissionIdeasService missionIdeasService,
            IMissionContentsService missionContentsService,
            IStoriesService storiesService,
            IImagesService imageService,
            IConfiguration configuration, ISchoolYearService schoolYearService)
        {
            _dbContext = dbContext;
            this._missionsService = missionsService;
            this._missionIdeasService = missionIdeasService;
            _missionContentsService = missionContentsService;
            this._storiesService = storiesService;
            this._imageService = imageService;
            this._configuration = configuration;
            _schoolYearService = schoolYearService;
            this._dateTimeFormat = this._configuration["DateТimeFormat"];
        }

        public IEnumerable<MissionIdeaViewModel> GetMissionIdeaViewModels()
        {
            var timeheroesMissions = this._missionIdeasService.GetAllPublishedMissionIdeas();
            return timeheroesMissions.Select(mi => this.MapMissionIdeaToMissionIdeaViewModel(mi));
        }

        public IEnumerable<MissionViewModel> GetMissionViewModels()
        {
            var missions = this._missionsService.GetAllPublishedMissions();
            return missions.Select(m => this.MapMissionToMissionViewModel(m, this._missionsService.GetMissionImagesIds(m.Id)));
        }

        public async Task<IEnumerable<MissionViewModel>> GetPinnedMissionViewModels()
        {
            var pinnedMissions = await this._missionsService.GetPinnedMissions();
            return pinnedMissions.Select(m => this.MapMissionToMissionViewModel(m, this._missionsService.GetMissionImagesIds(m.Id)));
        }

        public int GetAllMissionsCount()
        {
            return this._missionsService.GetAllPublishedMissions().Count();
        }

        public IDictionary<string, int> GetMissionsPerLocation()
        {
            return this._missionsService.GetAllPublishedMissions()
                .GroupBy(m => m.Location)
                .ToDictionary(x => x.Key, x => x.Count());
        }

        public IEnumerable<MissionViewModel> GetMissionViewModelsByLocation(string location)
        {
            return this._missionsService.GetAllPublishedMissions()
                .Where(m => m.Location.Contains(location) || location.Contains(m.Location))
                .Select(m => this.MapMissionToMissionViewModel(m, this._missionsService.GetMissionImagesIds(m.Id)));
        }

        public IEnumerable<StoryViewModel> GetAllPublishedStoryViewModels()
        {
            return this._storiesService.GetAllPublishedStories().Select(s => this.MapStoryToStoryViewModel(s));
        }

        public async Task<MissionViewModel> GetMissionViewModelBySlugAsync(string slug)
        {
            var result = await this._missionsService.GetMissionEditModelBySlugAsync(slug);
            if (result == null)
            {
                return null;
            }

            var model = this.MapMissionEditModelToMissionViewModel(result);

            return model;
        }

        public async Task<StoryViewModel> GetStoryViewModelByMissionSlugAsync(string missionSlug)
        {
            var story = await this._storiesService.GetStoryByMissionSlugAsync(missionSlug);
            if (story == null)
            {
                return null;
            }

            var model = this.MapStoryToStoryViewModel(story, true);

            return model;
        }

        public async Task<MissionIdeaViewModel> GetMissionIdeaViewModelBySlugAsync(string slug)
        {
            var result = await this._missionIdeasService.GetMissionIdeaEditModelBySlugAsync(slug);
            if (result == null)
            {
                return null;
            }

            var model = this.MapMissionIdeaEditModelToMissionIdeaViewModel(result);

            return model;
        }

        private MissionIdeaViewModel MapMissionIdeaToMissionIdeaViewModel(MissionIdea missionIdea)
        {
            if (missionIdea == null)
            {
                return null;
            }

            return new MissionIdeaViewModel()
            {
                Id = missionIdea.Id,
                Slug = missionIdea.Slug,
                MissionIdea = missionIdea,
                ImageId = missionIdea.MissionIdeaImages != null && missionIdea.MissionIdeaImages.Any() ? missionIdea.MissionIdeaImages.FirstOrDefault().ImageId.ToString() : null,
                IsExpired = missionIdea.EndDate.IsExpired(),
                IsSeveralDays = IsSeveralDays(missionIdea.StartDate, missionIdea.EndDate),
                Organization = !string.IsNullOrEmpty(missionIdea.Organization) ? missionIdea.Organization : this._configuration["DefaultOrganization"]
            };
        }

        private MissionIdeaViewModel MapMissionIdeaEditModelToMissionIdeaViewModel(MissionIdeaEditModel missionIdeEditModel)
        {
            if (missionIdeEditModel == null)
            {
                return null;
            }

            return new MissionIdeaViewModel()
            {
                Id = missionIdeEditModel.MissionIdea.Id,
                Slug = missionIdeEditModel.MissionIdea.Slug,
                ImageId = missionIdeEditModel.ImageId,
                ImageFilename = missionIdeEditModel.ImageFilename,
                MissionIdea = missionIdeEditModel.MissionIdea,
                StartDate = missionIdeEditModel.MissionIdea.StartDate.ConvertToLocalDateTime(),
                EndDate = missionIdeEditModel.MissionIdea.EndDate.ConvertToLocalDateTime(),
                IsExpired = missionIdeEditModel.MissionIdea.EndDate.IsExpired(),
                IsSeveralDays = IsSeveralDays(missionIdeEditModel.MissionIdea.StartDate, missionIdeEditModel.MissionIdea.EndDate),
                Organization = missionIdeEditModel.MissionIdea.Organization != null && missionIdeEditModel.MissionIdea.Organization != String.Empty ? missionIdeEditModel.MissionIdea.Organization : this._configuration["DefaultOrganization"]
            };
        }

        private StoryViewModel MapStoryToStoryViewModel(Story story, bool includeImages = false)
        {
            if (story == null)
            {
                return null;
            }

            var storyImageIds = story.StoryImages.Select(s => s.ImageId.ToString());
            string heroImageId = story.StoryImages != null && story.StoryImages.Any() ?
                story.StoryImages.FirstOrDefault()?.ImageId.ToString() :
                story.Mission.MissionImages.FirstOrDefault()?.ImageId.ToString();
            string heroImageFilename = null;

            if (includeImages)
            {
                heroImageFilename = story.StoryImages != null && story.StoryImages.Any() ?
                    story.StoryImages.FirstOrDefault().Image.Filename :
                    story.Mission.MissionImages.FirstOrDefault().Image.Filename;
            }


            return new StoryViewModel()
            {
                Id = story.Id,
                Content = story.Content,
                ClubName = story.Mission.Club.Name,
                HeroImageFilename = heroImageFilename,
                ImageIds = storyImageIds,
                HeroImageId = heroImageId,
                Mission = new MissionViewModel()
                {
                    Id = story.Mission.Id,
                    Title = story.Mission.Title,
                    Slug = story.Mission.Slug,
                    ClubName = story.Mission.Club.Name,
                    PostClubName = GetPostClubName(story.Mission.Club),
                    ClubLocation = story.Mission.Club.Location,
                    IsExpired = story.Mission.EndDate.IsExpired(),
                    IsSeveralDays = IsSeveralDays(story.Mission.StartDate, story.Mission.EndDate),
                    ImageFilename = this._imageService.GetImageFilename(story.Mission.MissionImages.FirstOrDefault() != null ? story.Mission.MissionImages.FirstOrDefault().Image : null),
                    ImageId = story.Mission.MissionImages != null && story.Mission.MissionImages.Any() ? story.Mission.MissionImages.FirstOrDefault().ImageId.ToString() : null,
                    StartDate = story.Mission.StartDate.ConvertToLocalDateTime(),
                    EndDate = story.Mission.EndDate.ConvertToLocalDateTime(),
                }
            };
        }

        private MissionViewModel MapMissionEditModelToMissionViewModel(MissionEditModel missionEditModel)
        {
            if (missionEditModel == null)
            {
                return null;
            }

            return new MissionViewModel()
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
                Story = missionEditModel.Mission.Story != null ? new StoryViewModel()
                {
                    Content = missionEditModel.Mission.Story != null ? missionEditModel.Mission.Story.Content : null,
                    ClubName = missionEditModel.Mission.Club.Name,
                    ImageIds = missionEditModel.Mission.Story.StoryImages != null && missionEditModel.Mission.Story.StoryImages.Any() ? missionEditModel.Mission.Story.StoryImages.Select(s => s.ImageId.ToString()) : null,
                } : null
            };
        }
      
        private MissionViewModel MapMissionToMissionViewModel(Mission mission, IEnumerable<Tuple<string, string>> missionImages)
        {
            if (mission == null)
            {
                return null;
            }

            return new MissionViewModel()
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
                Story = mission.Story != null ? new StoryViewModel()
                {
                    Content = mission.Story.Content,
                    ClubName = mission.Club.Name,
                    ImageIds = mission.Story.StoryImages != null && mission.Story.StoryImages.Any() ? mission.Story.StoryImages.Select(s => s.ImageId.ToString()) : null,
                } : null                
            };
        }

        private bool IsSeveralDays(long startDate, long endDate)
        {
            return endDate.ConvertToLocalDateTime().Date != startDate.ConvertToLocalDateTime().Date;
        }

        private string GetPostClubName(Club club)
        {
            var clubNumber = club.OrganizationNumber != null && club.OrganizationNumber != string.Empty ? $" {club.OrganizationNumber} " : string.Empty;
            var clubName = $"\"{club.Name}\", {clubNumber} {club.OrganizationType} \"{club.OrganizationName}\"";

            return clubName;
        }

        public string ParseLocation(string location)
        {
            if (location == null)
            {
                return null;
            }

            var parsedResult = Regex.Replace(location, @"(\(\d\))", "");
            return parsedResult.Trim();
        }
        public async Task<MissionListModel> GetMissionListModelAsync(Guid? ownerId)
        {
            var missions = new List<Mission>();
            missions = await this._dbContext.Missions
                    .Include(m => m.Club)
                    .Include(m => m.HeroMissions)
                    .ThenInclude(hm => hm.Hero)
                    .ToListAsync();

            if (ownerId.HasValue)
            {
                missions = missions.Where(m => m.Club.OwnerId == ownerId.Value).ToList();
            }

            var model = new MissionListModel()
            {
                Missions = missions
                                .OrderBy(m => m.IsPublished)
                                .ThenByDescending(m => m.UpdatedOn)
                                .Select(m => new MissionListItem()
                                {
                                    Id = m.Id,
                                    Title = m.Title,
                                    ClubId = m.ClubId,
                                    ClubName = m.Club.Name,
                                    HeroesCount = m.HeroMissions != null ? m.HeroMissions.Count(hm => hm.MissionId == m.Id) : 0,
                                    IsPublished = m.IsPublished,
                                    IsPinned = m.IsPinned,
                                    LastUpdateOn = m.UpdatedOn.ToUniversalDateTime().ToLocalTime().ToString(this._dateTimeFormat)
                                })

            };

            return model;
        }

        public async Task<MissionEditModel> CreateMissionEditModelAsync(Guid? ownerId)
        {
            var clubs = new List<Club>();
            clubs = await this._dbContext.Clubs.ToListAsync();

            if (ownerId.HasValue)
            {
                clubs = clubs.Where(c => c.OwnerId == ownerId.Value).ToList();
            }

            var newMission = new Mission();
            newMission.Content = new MissionContent();
            newMission.OwnerId = ownerId.HasValue ? ownerId.Value : Guid.Empty;

            var model = new MissionEditModel()
            {
                Mission = newMission,
                Clubs = clubs,
                ClubId = clubs.Count > 0 && newMission.OwnerId != Guid.Empty ? clubs.FirstOrDefault().Id : Guid.Empty
            };

            return model;
        }

        public async Task<Guid> SaveMissionEditModelAsync(MissionEditModel model)
        {
            var mission = await this._dbContext.Missions
                .Include(c => c.MissionImages)
                .ThenInclude(m => m.Image)
                .Include(m => m.Club)
                .Include(m => m.Content)
                .FirstOrDefaultAsync(m => m.Id == model.Mission.Id && m.OwnerId == model.Mission.OwnerId);

            var slug = model.Mission.Title.Trim().ToSlug();
            slug = slug.Unidecode();

            var missionWithSameTitle = await this._dbContext.Missions
                .Where(m => (m.Title == model.Mission.Title || m.Slug == slug) && m.Id != model.Mission.Id)
                .FirstOrDefaultAsync();

            if (missionWithSameTitle != null)
            {
                throw new ExistingItemException();
            }

            if (mission == null)
            {
                mission = new Mission();
                mission.Id = model.Mission.Id != Guid.Empty ? model.Mission.Id : Guid.NewGuid();
                var club = await this._dbContext.Clubs.FirstOrDefaultAsync(c => c.Id == model.Mission.Club.Id);
                mission.OwnerId = club.OwnerId;
                mission.CreatedOn = DateTime.Now.ToUnixMilliseconds();
                this._dbContext.Missions.Add(mission);
            }

            mission.Title = model.Mission.Title.Trim();
            mission.Slug = slug;
            mission.Location = model.Mission.Location;
            if (model.Mission.Stars != 0)
            {
                mission.Stars = model.Mission.Stars;
            }

            var dateFormat = this._configuration["DateFormat"];
            var startDate = DateTime.ParseExact(model.UploadedStartDate, dateFormat, CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(model.UploadedEndDate, dateFormat, CultureInfo.InvariantCulture);
            mission.StartDate = startDate.StartOfTheDay().ToUnixMilliseconds();
            mission.EndDate = endDate.EndOfTheDay().ToUnixMilliseconds();
            if (model.Mission.DurationInHours != 0)
            {
                mission.DurationInHours = model.Mission.DurationInHours;
            }
            mission.SchoolYear = this._schoolYearService.CalculateSchoolYear(startDate);
            await this._missionContentsService.SaveOrUpdateMissionContent(model.Mission.Content, mission);

            // set mission organizer
            if (model.Mission.Club.Id != Guid.Empty)
            {
                var newOrganizator = this._dbContext.Clubs.FirstOrDefault(h => h.Id == model.Mission.Club.Id);
                mission.Club = newOrganizator;
            }

            // set mission image
            if (model.Image != null)
            {
                var image = this._imageService.MapFormFileToImage(model.Image);
                await this._imageService.CreateMissionImageAsync(image, mission);
            }

            mission.UpdatedOn = DateTime.Now.ToUnixMilliseconds();

            await _dbContext.SaveChangesAsync();
            return mission.Id;
        }

        public async Task<bool> PublishMissionEditModelAsync(Guid missionId)
        {
            var mission = await this._dbContext.Missions.FirstOrDefaultAsync(m => m.Id == missionId);
            if (mission == null)
            {
                return false;
            }

            mission.IsPublished = true;
            await this._dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnPublishMissionEditModelAsync(Guid missionId)
        {
            var mission = await this._dbContext.Missions.FirstOrDefaultAsync(m => m.Id == missionId);
            if (mission == null)
            {
                return false;
            }

            mission.IsPublished = false;
            mission.IsPinned = false;
            await this._dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<MissionEditModel> GetMissionEditModelByIdAsync(Guid id, Guid? ownerId)
        {
            Mission mission = null;
            mission = await this._dbContext.Missions
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
            var mission = this._dbContext.Missions.FirstOrDefault(c => c.Id == id);
            if (mission == null)
            {
                return false;
            }

            this._dbContext.Missions.Remove(mission);
            await this._dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Mission>> GetMissionsBySchoolYear(string schoolYear)
        {
            if (string.IsNullOrEmpty(schoolYear) || string.IsNullOrWhiteSpace(schoolYear))
            {
                return null;
            }

            var result = await this._dbContext.Missions
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
            var schoolYears = this._dbContext.Missions
               .Where(m => m.IsPublished && m.Stars != 0 && m.HeroMissions != null && m.HeroMissions.Count > 0)
               .GroupBy(m => m.SchoolYear)
               .Select(sy => sy.Key);

            return schoolYears;
        }

        public IEnumerable<Mission> GetAllPublishedMissions()
        {
            var missions = this._dbContext.Missions
                .Where(m => m.IsPublished == true)
                .Include(m => m.HeroMissions)
                .Include(m => m.Club)
                .Include(m => m.Story)
                .OrderByDescending(m => m.StartDate);

            return missions.ToList();
        }

        public async Task<bool> PinMissionEditModelAsync(Guid id)
        {
            var mission = await this._dbContext.Missions.FirstOrDefaultAsync(m => m.Id == id);
            if (mission == null)
            {
                return false;
            }

            mission.IsPinned = true;
            await this._dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnpinMissionEditModelAsync(Guid id)
        {
            var mission = await this._dbContext.Missions.FirstOrDefaultAsync(m => m.Id == id);
            if (mission == null)
            {
                return false;
            }

            mission.IsPinned = false;
            await this._dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Mission>> GetPinnedMissions()
        {
            var missions = await this._dbContext.Missions
                .Where(m => m.IsPinned == true && m.IsPublished == true)
                .Include(m => m.Club)
                .Include(m => m.MissionImages)
                .Include(m => m.Story)
                .ThenInclude(s => s.StoryImages)
                .ToListAsync();

            int countOfPinnedMissionsOnHomePage;
            int.TryParse(this._configuration["PinnedMissionsOnHomePageCount"], out countOfPinnedMissionsOnHomePage);

            if (missions.Count() < countOfPinnedMissionsOnHomePage)
            {
                var countOfMissionsToAdd = countOfPinnedMissionsOnHomePage - missions.Count();
                var latestMissions = await this._dbContext.Missions
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
            if (commit)
            {
                await this._dbContext.SaveChangesAsync();
            }
        }

        public async Task SaveMissionHeroes(Mission mission, IEnumerable<Guid> heroesIds, bool commit = false)
        {
            // set missions's heroes
            if (heroesIds != null && heroesIds.Any())
            {
                await DeleteHeroMissions(mission);
                await AddHeroesToMission(mission, heroesIds, false);
            }
            else
            {
                await DeleteHeroMissions(mission);
            }

            if (commit)
            {
                await this._dbContext.SaveChangesAsync();
            }
        }

        private async Task DeleteHeroMissions(Mission mission, bool commit = false)
        {
            var heroMissions = this._dbContext.HeroMissions.Where(hm => hm.MissionId == mission.Id);
            foreach (var heroMission in heroMissions)
            {
                this._dbContext.HeroMissions.Remove(heroMission);
            }

            if (commit)
            {
                await this._dbContext.SaveChangesAsync();
            }
        }

        private async Task AddHeroesToMission(Mission mission, IEnumerable<Guid> heroesIds, bool commit = false)
        {
            var heroMissions = new List<HeroMission>();
            foreach (var heroId in heroesIds)
            {
                var hero = this._dbContext.Heroes.FirstOrDefault(h => h.Id == heroId);
                heroMissions.Add(new HeroMission()
                {
                    Hero = hero,
                    Mission = mission
                });
            }

            mission.HeroMissions = heroMissions;

            if (commit)
            {
                await this._dbContext.SaveChangesAsync();
            }
        }

        public async Task<MissionEditModel> GetMissionEditModelBySlugAsync(string slug)
        {
            Mission mission = null;
            mission = await this._dbContext.Missions
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

        private async Task<MissionEditModel> MapMissionToMissionEditModel(Mission mission)
        {
            if (mission == null)
            {
                return null;
            }

            if (mission.Content == null)
            {
                mission.Content = new MissionContent();
            }

            var model = await CreateMissionEditModelAsync(null);
            model.Mission = mission;
            model.ClubId = mission.Club != null && mission.Club.Id != Guid.Empty ? mission.Club.Id : Guid.Empty;

            if (mission.MissionImages != null && mission.MissionImages.Count > 0)
            {
                var missionImage = await this._imageService.GetMissionImage(mission.Id);
                model.ImageId = missionImage.ImageId.ToString();
                model.ImageFilename = missionImage.Image.Filename;
            }

            var dateFormat = this._configuration["DateFormat"];
            model.UploadedStartDate = mission.StartDate.ToUniversalDateTime().ToLocalTime().ToString(dateFormat);
            model.UploadedEndDate = mission.EndDate.ToUniversalDateTime().ToLocalTime().ToString(dateFormat);
            model.Duration = GetMissionDuration(mission.StartDate, mission.EndDate);

            return model;
        }

        public IEnumerable<Tuple<string, string>> GetMissionImagesIds(Guid missionId)
        {

            return this._dbContext.MissionImages
            .Include(mi=>mi.Image)
            .Where(m=>m.MissionId == missionId)
            .Select(mi=> new Tuple<string, string> (mi.ImageId.ToString(),mi.Image.Filename));
        }
    }
}
