using HeroesCup.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace HeroesCup.Web.Controllers;

public class SearchController : Controller
{
    private readonly IConfiguration _config;

    public SearchController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromForm] SearchViewModel model)
    {
        var googleReCaptchaResult = await RecaptchaValidator.Verify(_config, model.Token);
        if (googleReCaptchaResult)
        {
            SearchResponseViewModel responseViewModel;
            return View(responseViewModel);
        }

        return BadRequest();
    }
}

public class SearchResponseViewModel
{
    public string Title { get; set; }
    public string Date { get; set; }
    public string Text { get; set; }
    public string Author { get; set; }
    public string Status { get; set; }
}

public class SearchViewModel
{
    public string Search { get; set; }
    public string Token { get; set; }
}