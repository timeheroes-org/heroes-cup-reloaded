using HeroesCup.Web.Data;
using HeroesCup.Web.Models;
using Piranha;

namespace HeroesCup.Web.Services;

public class SearchService : ISearchServce
{
    private readonly IDb _db;
    private readonly HeroesCupDbContext _dbContext;
    private readonly IMissionsService _missionsService;

    public SearchService(IDb db, HeroesCupDbContext dbContext, IMissionsService missionsService)
    {
        _db = db;
        _dbContext = dbContext;
        _missionsService = missionsService;
    }

    public async Task<SearchResponseModel> Search(string searchTerm)
    {
        var response = new SearchResponseModel();
        var searchMissions = _missionsService.GetAllPublishedMissions()?.Where(m =>
            m.Title.Contains(searchTerm) || m.Content.Contact.Contains(searchTerm) ||
            m.Content.Equipment.Contains(searchTerm) || m.Content.What.Contains(searchTerm) ||
            m.Content.Where.Contains(searchTerm) || m.Content.Where.Contains(searchTerm) ||
            m.Content.Why.Contains(searchTerm)).ToList();
        if (searchMissions != null && searchMissions.Any())
        {
            response.Items.AddRange(searchMissions.Select(s => new SearchItem()
            {
                Author = s.Club.Name,
                Date = $@"{{s.StartDate}} - {s.EndDate}",
                Status = s.IsPublished ? "Публикувана" : "Приключила",
                Text = s.Content.What,
                Title = s.Title,
                Type = SearchResultType.Mission
            }));
        }

        return response;
    }
}