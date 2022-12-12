using HeroesCup.Web.Data;
using HeroesCup.Web.Models;
using HeroesCup.Web.Models.Events;
using Piranha;
using Piranha.Extend.Blocks;

namespace HeroesCup.Web.Services;

public class SearchService : ISearchServce
{
    private readonly IDb _db;
    private readonly HeroesCupDbContext _dbContext;
    private readonly IMissionsService _missionsService;
    private readonly IApi _api;
    private readonly IClubsService _clubsService;

    public SearchService(IDb db, HeroesCupDbContext dbContext, IMissionsService missionsService, IClubsService clubsService, IApi api)
    {
        _db = db;
        _dbContext = dbContext;
        _missionsService = missionsService;
        _api = api;
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
        var searchEvents = (await _api.Posts.GetAllAsync<EventPost>("events"))
            .Where(e =>
                e.Title.ToUpper().Contains(searchTerm) || 
                (e.Blocks.Count > 0 && e.Blocks[0] is HtmlBlock && 
                ((HtmlBlock)e.Blocks[0]).Body.Value.ToUpper().Contains(searchTerm)))
            .ToList();
        var clubs = await _clubsService.GetAllClubsWithImages();
        var searchClubs = clubs.FindAll(c => (!string.IsNullOrEmpty(c.Name) && c.Name.ToUpper().Contains(searchTerm)) ||
                                           (!string.IsNullOrEmpty(c.Description) && c.Description.ToUpper().Contains(searchTerm)) ||
                                           (!string.IsNullOrEmpty(c.OrganizationName) && c.OrganizationName.ToUpper().Contains(searchTerm))).ToList();
        if (searchMissions.Any())
        {
            response.Items.AddRange(searchMissions.Select(s => new SearchItem
            {
                Id = s.Slug,
                Author = s.Club?.Name,
                Date = $@"{new DateTime(s.StartDate).ToShortDateString()} - {new DateTime(s.EndDate).ToShortDateString()}",
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

        if (searchEvents.Any())
        {
            response.Items.AddRange(searchEvents.Select(e => new SearchItem
            {
                Author = e.Author,
                Date = e.Created.ToShortDateString(),
                Slug = e.Slug,
                Image = e.Hero?.PrimaryImage?.Media?.PublicUrl,
                Text = ((HtmlBlock)e.Blocks.FirstOrDefault(b=>b is HtmlBlock))?.Body,
                Title = e.Title,
                Type = SearchResultType.Event

            }).ToList());
            
        }
        return response;
    }
}