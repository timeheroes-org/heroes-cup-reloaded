using HeroesCup.Web.Common;
using HeroesCup.Web.Models;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HeroesCup.Web.Controllers;

[Route("[controller]")]
public class SearchController : Controller
{
    private readonly IConfiguration _config;
    private readonly ISearchServce _searchService;

    public SearchController(IConfiguration config, ISearchServce searchService)
    {
        _config = config;
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchPage([FromForm] SearchViewModel model)
    {
        ViewBag.IsSearchPage = true;
        return View("Search", new SearchResponseModel());
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromForm] SearchViewModel model)
    {
        ViewBag.IsSearchPage = true;
        var googleReCaptchaResult = await RecaptchaValidator.Verify(_config, model.Token);
        if (googleReCaptchaResult)
        {
            SearchResponseModel responseModel = await _searchService.Search(model.SearchTerm);
            ViewBag.SearchTerm = model.SearchTerm;
            return View(responseModel);
        }
        return View(new SearchResponseModel());
    }
}