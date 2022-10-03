using HeroesCup.Web.Data;
using HeroesCup.Web.Models;
using Piranha;

namespace HeroesCup.Web.Services;

public class SearchService : ISearchServce
{
    private readonly IDb _db;
    private readonly HeroesCupDbContext _dbContext;
    private readonly IMissionsService _missionsService;
    private readonly IClubsService _clubsService;

    public SearchService(IDb db, HeroesCupDbContext dbContext, IMissionsService missionsService, IClubsService clubsService)
    {
        _db = db;
        _dbContext = dbContext;
        _missionsService = missionsService;
        _clubsService = clubsService;
    }

    public async Task<SearchResponseModel> Search(string searchTerm)
    {
        searchTerm = searchTerm.ToUpper();
        var response = new SearchResponseModel();
        var publishedMissions = await _missionsService.GetAllPublishedMissionsWithContentAndImages();
        var searchMissions = publishedMissions.OrderByDescending(m => m.StartDate).Where(m =>
            m.Title.ToUpper().Contains(searchTerm) || m.Content.Contact.ToUpper().Contains(searchTerm) ||
            m.Content.Equipment.ToUpper().Contains(searchTerm) || m.Content.What.ToUpper().Contains(searchTerm) ||
            m.Content.Where.ToUpper().Contains(searchTerm) || m.Content.Where.ToUpper().Contains(searchTerm) ||
            m.Content.Why.ToUpper().Contains(searchTerm)).ToList();
        
        var clubs = await _clubsService.GetAllClubsWithImages().Result;
        var searchClubs = clubs.FindAll(c => (!string.IsNullOrEmpty(c.Name) && c.Name.ToUpper().Contains(searchTerm)) ||
                                           (!string.IsNullOrEmpty(c.Description) && c.Description.ToUpper().Contains(searchTerm)) ||
                                           (!string.IsNullOrEmpty(c.OrganizationName) && c.OrganizationName.ToUpper().Contains(searchTerm))).ToList();
        if (searchMissions.Any())
        {
            response.Items.AddRange(searchMissions.Select(s => new SearchItem
            {
                Id = s.Slug,
                Author = s.Club?.Name,
                Date = $@"{s.StartDate} - {s.EndDate}",
                Status = s.IsPublished ? "Публикувана" : "Приключила",
                Text = s.Content?.What,
                Title = s.Title,
                Type = SearchResultType.Mission,
                Image = s.MissionImages?.FirstOrDefault()?.ImageId.ToString()
            }).ToList());
        }

        if (searchClubs.Any())
        {
            response.Items.AddRange(searchClubs.Select(s => new SearchItem
            {
                Id = s.Id.ToString(),
                Author = s.OrganizationName,
                Date = string.Empty,
                Status = $@"{s.Points} точки, {(s.Heroes == null ? "0" : s.Heroes.Count.ToString())} герои",
                Text = s.Description,
                Title = s.Name,
                Type = SearchResultType.Club,
                Image = s.ClubImages?.FirstOrDefault()?.ImageId.ToString()
            }).ToList());
        }

        return response;
    }
}