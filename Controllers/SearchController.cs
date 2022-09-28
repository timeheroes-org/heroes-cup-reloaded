using HeroesCup.Web.Common;
using HeroesCup.Web.Models;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HeroesCup.Web.Controllers;

public class SearchController : Controller
{
    private readonly IConfiguration _config;
    private readonly ISearchServce _searchServce;

    public SearchController(IConfiguration config, ISearchServce searchServce)
    {
        _config = config;
        _searchServce = searchServce;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromForm] SearchViewModel model)
    {
        var googleReCaptchaResult = await RecaptchaValidator.Verify(_config, model.Token);
        if (googleReCaptchaResult)
        {
            SearchResponseModel responseModel = await _searchServce.Search(model.SearchTerm);
            return View(responseModel);
        }

        return BadRequest();
    }
}