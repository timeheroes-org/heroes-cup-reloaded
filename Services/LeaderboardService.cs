using System.Text.RegularExpressions;
using HeroesCup.Data.Models;
using HeroesCup.Web.Common;
using HeroesCup.Web.Common.Extensions;
using HeroesCup.Web.Models;

namespace HeroesCup.Web.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IImagesService _imagesService;
    private readonly IMissionsService _missionsService;

    public LeaderboardService(IMissionsService missionsService,
        IImagesService imagesService)
    {
        _missionsService = missionsService;
        _imagesService = imagesService;
    }

    public async Task<ClubListViewModel> GetClubsBySchoolYearAsync(string schoolYear)
    {
        var missions = await _missionsService.GetMissionsBySchoolYear(schoolYear);
        if (missions == null) return null;

        var clubs = missions
            .GroupBy(m => m.Club)
            .Select(g => new
            {
                Club = g.Key,
                Missions = g.ToList()
            })
            .Select(c =>
            {
                var clubMissions = c.Club.Missions
                    .OrderByDescending(m => m.StartDate)
                    .Select(m => new MissionViewModel
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Club = m.Club,
                        ImageId = GetMissionImageId(m),
                        Slug = m.Slug,
                        EndDate = m.EndDate.ConvertToLocalDateTime(),
                        StartDate = m.StartDate.ConvertToLocalDateTime(),
                        IsExpired = m.EndDate.IsExpired()
                    });

                var clubHeroes = c.Club.Heroes.Select(h => new HeroViewModel
                {
                    HeroInitials = GetClubInitials(h.Name),
                    IsCoordinator = h.IsCoordinator,
                    Name = h.Name
                });

                return new ClubListItem
                {
                    Id = c.Club.Id,
                    Name = GetClubName(c.Club),
                    Location = c.Club.Location,
                    ClubInitials = GetClubInitials(c.Club.Name),
                    HeroesCount = GetHeroesCount(c.Club),
                    ClubImageId = _imagesService.getClubImageId(c.Club.Id),
                    Points = getClubPoints(c.Missions),
                    Club = c.Club,
                    Missions = clubMissions,
                    Heroes = clubHeroes,
                    Coordinators = clubHeroes.Where(h => h.IsCoordinator)
                };
            })
            .OrderByDescending(c => c.Points)
            .ThenBy(c => c.Club.Name);

        var model = new ClubListViewModel
        {
            Clubs = clubs
        };

        return model;
    }

    public IEnumerable<string> GetSchoolYears()
    {
        return _missionsService.GetMissionSchoolYears().OrderBy(x => x);
    }

    public string GetLatestSchoolYear()
    {
        var latestSchoolYear = GetSchoolYears().MaxBy(x => x);
        return latestSchoolYear;
    }

    private string GetMissionImageId(Mission mission)
    {
        var missionImagesIds = _missionsService.GetMissionImagesIds(mission.Id);
        if (missionImagesIds != null && missionImagesIds.Any()) return missionImagesIds.FirstOrDefault().Item1;

        return null;
    }

    private string GetClubName(Club club)
    {
        return $"Клуб \"{club.Name}\", {club.OrganizationNumber} {club.OrganizationType} \"{club.OrganizationName}\"";
    }

    private string GetClubInitials(string organizationName)
    {
        var name = organizationName;
        var charactersToTrim = new[] { ' ', '*', '.', '"', '\'', '”', '“' };
        var initialsReg = new Regex(@"(\b[a-zA-Z-а-яА-Я])[a-zA-Z-а-яА-Я]* ?");
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
        
        return missions.Where(m=>new DateTime(2022, 9, 15).ToUnixMilliseconds() >= m.StartDate).Select(m => m.Stars * m.HeroMissions.Count()).Sum() +
               missions.Where(m=>new DateTime(2022, 9, 15).ToUnixMilliseconds() < m.StartDate).Sum(s=> s.Stars +
                   (s.HeroMissions.Count is >= 50 and < 100 ? 1
                       : s.HeroMissions.Count >= 100 ? 2 : 0));
    }
}