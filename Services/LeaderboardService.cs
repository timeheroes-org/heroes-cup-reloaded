using HeroesCup.Data.Models;
using HeroesCup.Web.Models;
using System.Text.RegularExpressions;
using HeroesCup.Web.Common;
using HeroesCup.Web.Common.Extensions;

namespace HeroesCup.Web.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly IMissionsService _missionsService;
        private readonly IImagesService _imagesService;

        public LeaderboardService(IMissionsService missionsService,
            IImagesService imagesService)
        {
            this._missionsService = missionsService;
            this._imagesService = imagesService;
        }

        public async Task<ClubListViewModel> GetClubsBySchoolYearAsync(string schoolYear)
        {
            var missions = await this._missionsService.GetMissionsBySchoolYear(schoolYear);
            if (missions == null)
            {
                return null;
            }

            var clubs = missions
                .GroupBy(m => m.Club)
                .Select(g => new
                {
                    Club = g.Key,
                    Missions = g.ToList()
                })
                .Select(c =>
                {
                    IEnumerable<MissionViewModel> clubMissions = c.Club.Missions
                    .OrderByDescending(m => m.StartDate)
                    .Select(m => new MissionViewModel()
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Club = m.Club,
                        ImageId = this.GetMissionImageId(m),
                        Slug = m.Slug,
                        EndDate = m.EndDate.ConvertToLocalDateTime(),
                        StartDate = m.StartDate.ConvertToLocalDateTime(),
                        IsExpired = m.EndDate.IsExpired()
                    });

                    IEnumerable<HeroViewModel> clubHeroes = c.Club.Heroes.Select(h => new HeroViewModel()
                    {
                        HeroInitials = GetClubInitials(h.Name),
                        IsCoordinator = h.IsCoordinator,
                        Name = h.Name
                    });

                    return new ClubListItem()
                    {
                        Id = c.Club.Id,
                        Name = GetClubName(c.Club),
                        Location = c.Club.Location,
                        ClubInitials = GetClubInitials(c.Club.Name),                      
                        HeroesCount = GetHeroesCount(c.Club),
                        ClubImageId = this._imagesService.getClubImageId(c.Club.Id),
                        Points = getClubPoints(c.Missions),
                        Club = c.Club,
                        Missions = clubMissions,
                        Heroes = clubHeroes,
                        Coordinators = clubHeroes.Where(h => h.IsCoordinator)
                    };
                })
                .OrderByDescending(c => c.Points)
                .ThenBy(c => c.Club.Name);

            var model = new ClubListViewModel()
            {
                Clubs = clubs
            };

            return model;
        }

        private string GetMissionImageId(Mission mission)
        {
            var missionImagesIds = this._missionsService.GetMissionImagesIds(mission.Id);
            if (missionImagesIds != null && missionImagesIds.Any())
            {
                return missionImagesIds.FirstOrDefault().Item1;
            }

            return null;
        }

        private string GetClubName(Club club)
        {
            return $"Клуб \"{club.Name}\", {club.OrganizationNumber } {club.OrganizationType } \"{club.OrganizationName }\"";
        }

        private string GetClubInitials(string organizationName)
        {
            var name = organizationName;
            var charactersToTrim = new Char[] { ' ', '*', '.', '"', '\'', '”', '“' };
            Regex initialsReg = new Regex(@"(\b[a-zA-Z-а-яА-Я])[a-zA-Z-а-яА-Я]* ?");
            name = name.Trim(charactersToTrim).ToUpper();
            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2)
                .Select(w => w.Trim(charactersToTrim))
                .ToList();
            var result = string.Join(' ', words);
            var initialsResult = initialsReg.Replace(result, "$1");
            return initialsResult.Length > 3 ? initialsResult.Substring(0, 3) : initialsResult;
        }

        private int GetHeroesCount(Club club)
        {
            return club.Heroes.Count;
        }

        private int getClubPoints(IEnumerable<Mission> missions)
        {
            return missions.Select(m => m.Stars * m.HeroMissions.Count()).Sum();
        }

        public IEnumerable<string> GetSchoolYears()
        {
            return this._missionsService.GetMissionSchoolYears().OrderBy(x => x);
        }

        public string GetLatestSchoolYear()
        {
            var latestSchoolYear = this.GetSchoolYears().MaxBy(x => x);
            return latestSchoolYear;
        }
    }
}