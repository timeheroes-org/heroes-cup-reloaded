using HeroesCup.Web.Models;

namespace HeroesCup.Web.Services;

public interface ILeaderboardService
{
    string GetLatestSchoolYear();

    IEnumerable<string> GetSchoolYears();

    ClubListViewModel GetClubsBySchoolYearAsync(string schoolYear);
}