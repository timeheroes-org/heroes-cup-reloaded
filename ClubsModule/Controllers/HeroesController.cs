using HeroesCup.Localization;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.ClubsModule.Security;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Controllers;

namespace HeroesCup.Web.ClubsModule.Controllers;

public class HeroesController : ManagerController
{
    private readonly ManagerLocalizer heroesCupLocalizer;
    private readonly IHeroesService heroesService;
    private readonly IUserManager userManager;
    private readonly Guid? loggedInUserId;

    public HeroesController(IHeroesService heroesService, IUserManager userManager, ManagerLocalizer heroesCupLocalizer)
    {
        this.heroesService = heroesService;
        this.userManager = userManager;
        loggedInUserId = this.userManager.GetCurrentUserId();
        this.heroesCupLocalizer = heroesCupLocalizer;
    }

    [HttpGet]
    [Route("/manager/heroes")]
    [Authorize(Policy = Permissions.Heroes)]
    public async Task<IActionResult> ListAsync()
    {
        var model = await heroesService.GetHeroListModelAsync(loggedInUserId);
        return View(model);
    }

    [HttpGet]
    [Route("/manager/hero")]
    [Authorize(Policy = Permissions.HeroesAdd)]
    public async Task<IActionResult> Add()
    {
        var model = await heroesService.CreateHeroEditModelAsync(loggedInUserId);
        return View("Edit", model);
    }

    [HttpGet]
    [Route("/manager/hero/{id:Guid}")]
    [Authorize(Policy = Permissions.HeroesEdit)]
    public async Task<IActionResult> EditAsync(Guid id)
    {
        var model = await heroesService.GetHeroEditModelByIdAsync(id, loggedInUserId);
        if (model == null)
        {
            ErrorMessage(heroesCupLocalizer.Hero["The hero could not be found."], false);
            return RedirectToAction("List");
        }

        return View(model);
    }

    [HttpPost]
    [Route("/manager/hero/save")]
    [Authorize(Policy = Permissions.HeroesSave)]
    public async Task<IActionResult> SaveAsync(HeroEditModel model)
    {
        var heroId = await heroesService.SaveHeroEditModelAsync(model);
        if (heroId != Guid.Empty)
        {
            SuccessMessage(heroesCupLocalizer.Hero["The hero has been saved."]);
            return RedirectToAction("List");
        }

        ErrorMessage(heroesCupLocalizer.Hero["The hero could not be saved."], false);
        return View("Edit", model);
    }

    [HttpGet]
    [Route("/manager/hero/delete")]
    [Authorize(Policy = Permissions.ClubsDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await heroesService.DeleteAsync(id);
        if (!result)
        {
            ErrorMessage(heroesCupLocalizer.Hero["The hero could not be deleted."], false);
            return RedirectToAction("List");
        }

        SuccessMessage(heroesCupLocalizer.Hero["The hero has been deleted."]);
        return RedirectToAction("List");
    }
}