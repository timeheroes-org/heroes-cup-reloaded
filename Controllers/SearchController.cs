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

    [HttpPost]
    public async Task<IActionResult> Search([FromForm] SearchViewModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Token))
            return BadRequest();
        var googleReCaptchaResult = await RecaptchaValidator.Verify(_config, model.Token);
        if (googleReCaptchaResult)
        {
            SearchResponseModel responseModel = await _searchService.Search(model.SearchTerm);
            return View(responseModel);
        }

        return View(new SearchResponseModel());
    }
}