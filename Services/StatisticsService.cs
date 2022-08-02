namespace HeroesCup.Web.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IMissionsService _missionsService;
        private readonly IClubsService _clubsService;

        public StatisticsService(IClubsService clubsService, IMissionsService missionsService)
        {
            this._clubsService = clubsService;
            this._missionsService = missionsService;
            this._clubsService = clubsService;
        }

        public int GetAllClubsCount()
        {
            return this._clubsService.GetAllClubs().Count();
        }

        public int GetAllHeroesCount()
        {
            var clubs = this._clubsService.GetAllClubs();
            var heroesCount = 0;
            foreach (var club in clubs)
            {
                heroesCount += club.Heroes.Count();
            }

            return heroesCount;
        }

        public int GetAllHoursCount()
        {
            var missions = this._missionsService.GetAllPublishedMissions();
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
            return this._missionsService.GetAllPublishedMissions().Count();
        }
    }
}