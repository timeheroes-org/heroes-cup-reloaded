namespace HeroesCup.Web.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IClubsService _clubsService;
    private readonly IMissionsService _missionsService;

    public StatisticsService(IClubsService clubsService, IMissionsService missionsService)
    {
        _clubsService = clubsService;
        _missionsService = missionsService;
        _clubsService = clubsService;
    }

    public int GetAllClubsCount()
    {
        return _clubsService.GetAllClubs().Count();
    }

    public int GetAllHeroesCount()
    {
        var clubs = _clubsService.GetAllClubs();
        var heroesCount = 0;
        foreach (var club in clubs) heroesCount += club.Heroes.Count();

        return heroesCount;
    }

    public int GetAllHoursCount()
    {
        var missions = _missionsService.GetAllPublishedMissions();
        var hours = 0;
        foreach (var mission in missions)
        {
            var missionHours = mission.DurationInHours * mission.HeroMissions.Count();
            hours += missionHours;
        }

        return hours;
    }

    public int GetAllMissionsCount()
    {
        return _missionsService.GetAllPublishedMissions().Count();
    }
}