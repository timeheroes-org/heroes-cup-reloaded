using System.Text.RegularExpressions;
using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.Common;
using HeroesCup.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroesCup.Web.Services 
{
    public class StoriesService : IStoriesService
    {
        private readonly HeroesCupDbContext _dbContext;
        private readonly IImagesService _imagesService;
        private readonly IMissionsService _missionsService;
        private readonly IHeroesService _heroesService;
        private readonly IConfiguration _configuration;

        private string dateTimeFormat;

        public StoriesService(HeroesCupDbContext dbContext,
            IImagesService imagesService,
            IMissionsService missionsService,
            IHeroesService heroesService,
            IConfiguration configuration)
        {
            this._dbContext = dbContext;
            this._imagesService = imagesService;
            this._missionsService = missionsService;
            this._heroesService = heroesService;
            this._configuration = configuration;
            this.dateTimeFormat = this._configuration["DateТimeFormat"];
        }

        public async Task<StoryListModel> GetStoryListModelAsync(Guid? ownerId)
        {
            var stories = new List<Story>();
            stories = await this._dbContext.Stories
                .Include(c => c.Mission)
                .ThenInclude(m => m.Club)
                .ToListAsync();

            if (ownerId.HasValue)
            {
                stories = stories.Where(c => c.Mission.OwnerId == ownerId.Value).ToList();
            }

            var model = new StoryListModel()
            {
                Stories = stories
                    .OrderBy(s => s.IsPublished)
                    .ThenByDescending(s => s.UpdatedOn)
                    .Select(s => new StoryListItem()
                    {
                        Id = s.Id,
                        StartText = GetShortIntroText(s.Content, 50),
                        Mission = s.Mission,
                        IsPublished = s.IsPublished,
                        LastUpdateOn = s.UpdatedOn.ToUniversalDateTime().ToLocalTime().ToString(this.dateTimeFormat)
                    })

            };

            return model;
        }

        public async Task<StoryEditModel> CreateStoryEditModelAsync(Guid? ownerId)
        {
            var missions = new List<Mission>();
            ICollection<Hero> heroes = new List<Hero>();

            if (ownerId.HasValue)
            {
                missions = await this._dbContext.Missions
                    .Include(m => m.Story)
                    .Where(m => m.OwnerId == ownerId.Value && m.Story == null)
                    .ToListAsync();
                heroes = await this._heroesService.GetHeroes(null, ownerId);
            }
            else
            {
                missions = await this._dbContext.Missions
                    .Include(m => m.Story)
                    .Where(m => m.Story == null)
                    .ToListAsync();
            }

            var model = new StoryEditModel()
            {
                Story = new Story(),
                Missions = missions != null ? missions : new List<Mission>(),
                Heroes = heroes,
                HeroesIds = new List<Guid>()
            };

            return model;
        }

        public async Task<StoryEditModel> GetStoryEditModelByIdAsync(Guid id, Guid? ownerId)
        {
            var story = await this._dbContext.Stories
                .Include(s => s.Mission)
                .ThenInclude(m => m.Club)
                .Include(s => s.Mission)
                .ThenInclude(m => m.HeroMissions)
                .ThenInclude(m => m.Hero)
                .Include(c => c.StoryImages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (story == null)
            {
                return null;
            }

            if (ownerId.HasValue && story.Mission.Club.OwnerId != ownerId)
            {
                return null;
            }

            // var model = await CreateStoryEditModelAsync(ownerId);
            var model = new StoryEditModel();
            model.Story = story;
            model.Missions = new List<Mission>() { story.Mission };
            model.Heroes = await this._heroesService.GetHeroes(story.Mission.Club.Id, ownerId);
            model.HeroesIds = new List<Guid>();
            model.ImageIds = story.StoryImages != null && story.StoryImages.Any() ? story.StoryImages.Select(si => si.ImageId.ToString()).ToList() : new List<string>();

            if (story.Mission.HeroMissions != null && story.Mission.HeroMissions.Count > 0)
            {
                foreach (var heroMission in story.Mission.HeroMissions)
                {
                    var hero = await this._heroesService.GetHeroById(heroMission.HeroId);
                    model.HeroesIds.Add(hero.Id);
                }
            }

            return model;
        }

        public async Task<bool> PublishStoryEditModelAsync(Guid storyId)
        {
            var story = await this._dbContext.Stories.FirstOrDefaultAsync(m => m.Id == storyId);
            if (story == null)
            {
                return false;
            }

            story.IsPublished = true;
            await this._dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnpublishStoryEditModelAsync(Guid storyId)
        {
            var story = await this._dbContext.Stories.FirstOrDefaultAsync(m => m.Id == storyId);
            if (story == null)
            {
                return false;
            }

            story.IsPublished = false;
            await this._dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Guid> SaveStoryEditModelAsync(StoryEditModel model)
        {
            var story = await this._dbContext.Stories
                .Include(c => c.Mission)
                .Include(c => c.StoryImages)
                .FirstOrDefaultAsync(h => h.Id == model.Story.Id);

            if (story == null)
            {
                story = new Story();
                story.Id = model.Story.Id != Guid.Empty ? model.Story.Id : Guid.NewGuid();
                story.CreatedOn = DateTime.Now.ToUnixMilliseconds();
                this._dbContext.Stories.Add(story);
            }

            if (model.Story.MissionId != Guid.Empty)
            {
                story.Mission = this._dbContext.Missions.FirstOrDefault(m => m.Id == model.Story.MissionId);
            }

            story.Content = model.Story.Content;
            await this._missionsService.SaveMissionDurationHours(story.Mission, model.Story.Mission.DurationInHours);
            await this._missionsService.SaveMissionHeroes(story.Mission, model.HeroesIds);

            // set story image
            if (model.UploadedImages != null)
            {
                var images = new List<Image>();
                foreach (var uploadedImage in model.UploadedImages)
                {
                    var image = this._imagesService.MapFormFileToImage(uploadedImage);
                    images.Add(image);
                }

                await this._imagesService.CreateStoryImagesAsync(images, story);
            }

            story.UpdatedOn = DateTime.Now.ToUnixMilliseconds();

            await _dbContext.SaveChangesAsync();
            return story.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var story = this._dbContext.Stories.FirstOrDefault(c => c.Id == id);
            if (story == null)
            {
                return false;
            }

            var mission = this._dbContext.Missions
                .Include(m => m.HeroMissions)
                .FirstOrDefault(m => m.Story.Id == id);
            mission.HeroMissions = new List<HeroMission>();

            this._dbContext.Stories.Remove(story);
            await this._dbContext.SaveChangesAsync();
            return true;
        }

        private string GetShortIntroText(string htmlString, int length)
        {
            var text = GetPlainTextFromHtmlString(htmlString);
            text = GetShortTextFromString(text, length);

            return text + "...";
        }

        private string GetPlainTextFromHtmlString(string htmlString)
        {
            return Regex.Replace(htmlString, @"<(.|\n)*?>", "");
        }

        private string GetShortTextFromString(string htmlString, int length)
        {
            return htmlString.Substring(0, Math.Min(htmlString.Length, length));
        }

        public IEnumerable<Story> GetAllPublishedStories()
        {
            return this._dbContext.Stories
                .Include(s => s.Mission)
                .ThenInclude(m => m.MissionImages)
                .Include(s => s.Mission)
                .ThenInclude(m => m.Club)
                .Include(s => s.StoryImages)
                .Where(s => s.IsPublished == true)
                .OrderByDescending(s => s.Mission.StartDate);
        }

        public async Task<Story> GetStoryByMissionSlugAsync(string missionSlug)
        {
            return await this._dbContext.Stories
                .Include(s => s.Mission)
                .ThenInclude(m => m.MissionImages)
                .ThenInclude(mi => mi.Image)
                .Include(s => s.Mission)
                .ThenInclude(m => m.Club)
                .Include(s => s.StoryImages)
                .ThenInclude(si => si.Image)
                .FirstOrDefaultAsync(s => s.Mission.Slug == missionSlug);
        }
    }
}