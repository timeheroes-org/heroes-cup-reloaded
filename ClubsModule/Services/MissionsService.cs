﻿using ClubsModule.Common;
using ClubsModule.Exceptions;
using ClubsModule.Models;
using ClubsModule.Services.Contracts;
using HeroesCup.Data;
using HeroesCup.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HeroesCup.Web.Data;

namespace ClubsModule.Services
{
    public class MissionsService : IMissionsService
    {
        private readonly HeroesCupDbContext _dbContext;
        private readonly IImagesService _imagesService;
        protected readonly ISchoolYearService schoolYearService;
        private readonly IConfiguration _configuration;
        protected readonly IMissionContentsService missionContentsService;

        private readonly string dateTimeFormat;

        public MissionsService(HeroesCupDbContext dbContext,
            IImagesService imagesService,
            ISchoolYearService schoolYearService,
            IConfiguration configuration,
            IMissionContentsService missionContentsService)
        {
            this._dbContext = dbContext;
            this._imagesService = imagesService;
            this.schoolYearService = schoolYearService;
            this._configuration = configuration;
            this.missionContentsService = missionContentsService;
            this.dateTimeFormat = this._configuration["DateТimeFormat"];
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
                                    HeroesCount = m.HeroMissions != null ? m.HeroMissions.Where(hm => hm.MissionId == m.Id).Count() : 0,
                                    IsPublished = m.IsPublished,
                                    IsPinned = m.IsPinned,
                                    LastUpdateOn = m.UpdatedOn.ToUniversalDateTime().ToLocalTime().ToString(this.dateTimeFormat)
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
            mission.SchoolYear = this.schoolYearService.CalculateSchoolYear(startDate);
            await this.missionContentsService.SaveOrUpdateMissionContent(model.Mission.Content, mission);

            // set mission organizer
            if (model.Mission.Club.Id != Guid.Empty)
            {
                var newOrganizator = this._dbContext.Clubs.FirstOrDefault(h => h.Id == model.Mission.Club.Id);
                mission.Club = newOrganizator;
            }

            // set mission image
            if (model.Image != null)
            {
                var image = this._imagesService.MapFormFileToImage(model.Image);
                await this._imagesService.CreateMissionImageAsync(image, mission);
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

        public async Task<bool> UnpublishMissionEditModelAsync(Guid missionId)
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
                var missionImage = await this._imagesService.GetMissionImage(mission.Id);
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