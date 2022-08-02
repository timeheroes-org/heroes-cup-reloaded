using HeroesCup.Models;
using HeroesCup.Web.Models;
using HeroesCup.Web.Models.Events;
using HeroesCup.Web.Models.Resources;
using Piranha;

namespace HeroesCup.Web.Services;

public class PageInitializer : IPageInitializer
{
    private readonly IApi _api;
    private readonly IConfiguration _configuration;

    public PageInitializer(IApi api, IConfiguration configuration)
    {
        _api = api;
        _configuration = configuration;
    }

    public async Task SeedStarPageAsync()
    {
        var site = await _api.Sites.GetDefaultAsync();
        var pages = await _api.Pages.GetAllAsync();
        var startPage = pages.ToList().FirstOrDefault(p => p.TypeId == "StartPage");
        if (startPage == null)
        {
            var newStartPage = await StartPage.CreateAsync(_api);
            newStartPage.Id = Guid.NewGuid();
            newStartPage.SiteId = site.Id;
            newStartPage.Title = _configuration["StartPagePageSettings:Title"];
            newStartPage.Slug = _configuration["StartPagePageSettings:Slug"];
            newStartPage.MetaKeywords = _configuration["StartPagePageSettings:MetaKeywords"];
            newStartPage.MetaDescription = _configuration["StartPagePageSettings:MetaDescription"];
            newStartPage.NavigationTitle = _configuration["StartPagePageSettings:NavigationTitle"];
            newStartPage.Published = DateTime.Now;
            await _api.Pages.SaveAsync(newStartPage);
        }
    }

    public async Task SeedAboutPageAsync()
    {
        var site = await _api.Sites.GetDefaultAsync();
        var pages = await _api.Pages.GetAllAsync();
        var aboutPageTitle = _configuration["AboutPageSettings:Title"];
        var aboutPageSlug = _configuration["AboutPageSettings:Slug"];
        var aboutPage = pages.ToList().FirstOrDefault(p => p.Title == aboutPageTitle && p.Slug == aboutPageSlug);
        if (aboutPage == null)
        {
            var newAboutPage = await AboutPage.CreateAsync(_api);
            newAboutPage.Id = Guid.NewGuid();
            newAboutPage.SiteId = site.Id;
            newAboutPage.Title = aboutPageTitle;
            newAboutPage.Slug = _configuration["AboutPageSettings:Slug"];
            newAboutPage.MetaKeywords = _configuration["AboutPageSettings:MetaKeywords"];
            newAboutPage.MetaDescription = _configuration["AboutPageSettings:MetaDescription"];
            newAboutPage.NavigationTitle = _configuration["AboutPageSettings:NavigationTitle"];
            ;
            newAboutPage.Published = DateTime.Now;
            await _api.Pages.SaveAsync(newAboutPage);
        }
    }

    public async Task SeedResourcesPageAsync()
    {
        var site = await _api.Sites.GetDefaultAsync();
        var pages = await _api.Pages.GetAllAsync();
        var resourcesPage = pages.ToList().FirstOrDefault(p => p.TypeId == "ResourcesArchive");
        if (resourcesPage == null)
        {
            var newResourcesPage = await ResourcesArchive.CreateAsync(_api);
            newResourcesPage.Id = Guid.NewGuid();
            newResourcesPage.SiteId = site.Id;
            newResourcesPage.Title = _configuration["ResourcesPageSettings:Title"];
            newResourcesPage.Slug = _configuration["ResourcesPageSettings:Slug"];
            newResourcesPage.MetaKeywords = _configuration["ResourcesPageSettings:MetaKeywords"];
            newResourcesPage.MetaDescription = _configuration["ResourcesPageSettings:MetaKeywords"];
            newResourcesPage.NavigationTitle = _configuration["ResourcesPageSettings:NavigationTitle"];
            newResourcesPage.Published = DateTime.Now;
            await _api.Pages.SaveAsync(newResourcesPage);
        }
    }

    public async Task SeedEventsPageAsync()
    {
        var site = await _api.Sites.GetDefaultAsync();
        var pages = await _api.Pages.GetAllAsync();
        var eventsPage = pages.ToList().FirstOrDefault(p => p.TypeId == "EventsArchive");
        if (eventsPage == null)
        {
            var newEventsPage = await EventsArchive.CreateAsync(_api);
            newEventsPage.Id = Guid.NewGuid();
            newEventsPage.SiteId = site.Id;
            newEventsPage.Title = _configuration["EventsPageSettings:Title"];
            newEventsPage.Slug = _configuration["EventsPageSettings:Slug"];
            newEventsPage.MetaKeywords = _configuration["EventsPageSettings:MetaKeywords"];
            newEventsPage.MetaDescription = _configuration["EventsPageSettings:MetaKeywords"];
            newEventsPage.NavigationTitle = _configuration["EventsPageSettings:NavigationTitle"];
            newEventsPage.Published = DateTime.Now;
            await _api.Pages.SaveAsync(newEventsPage);
        }
    }

    public async Task SeedMissionsPageAsync()
    {
        var site = await _api.Sites.GetDefaultAsync();
        var pages = await _api.Pages.GetAllAsync();
        var eventsPage = pages.ToList().FirstOrDefault(p => p.TypeId == "MissionsPage");
        if (eventsPage == null)
        {
            var newMissionsPage = await MissionsPage.CreateAsync(_api);
            newMissionsPage.Id = Guid.NewGuid();
            newMissionsPage.SiteId = site.Id;
            newMissionsPage.Title = _configuration["MissionsPageSettings:Title"];
            newMissionsPage.Slug = _configuration["MissionsPageSettings:Slug"];
            newMissionsPage.MetaKeywords = _configuration["MissionsPageSettings:MetaKeywords"];
            newMissionsPage.MetaDescription = _configuration["MissionsPageSettings:MetaKeywords"];
            newMissionsPage.NavigationTitle = _configuration["MissionsPageSettings:NavigationTitle"];
            newMissionsPage.Published = DateTime.Now;
            await _api.Pages.SaveAsync(newMissionsPage);
        }
    }
}