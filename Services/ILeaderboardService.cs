using HeroesCup.Web.Models;

namespace HeroesCup.Web.Services;

public interface ILeaderboardService
{
    string GetLatestSchoolYear();

    IEnumerable<string> GetSchoolYears();

    Task<ClubListViewModel> GetClubsBySchoolYearAsync(string schoolYear);
}