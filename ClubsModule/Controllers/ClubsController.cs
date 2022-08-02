using HeroesCup.Localization;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.ClubsModule.Security;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Controllers;

namespace HeroesCup.Web.ClubsModule.Controllers;

public class ClubsController : ManagerController
{
    private readonly IClubsService clubsService;
    private readonly ManagerLocalizer heroesCupLocalizer;
    private readonly IUserManager userManager;
    private readonly Guid? loggedInUserId;

    public ClubsController(IClubsService clubsService, IUserManager userManager, ManagerLocalizer heroesCupLocalizer)
    {
        this.clubsService = clubsService;
        this.userManager = userManager;
        loggedInUserId = this.userManager.GetCurrentUserId();
        this.heroesCupLocalizer = heroesCupLocalizer;
    }

    [HttpGet]
    [Route("/manager/clubs")]
    [Authorize(Policy = Permissions.Clubs)]
    public async Task<IActionResult> ListAsync()
    {
        var model = await clubsService.GetClubListModelAsync(loggedInUserId);
        return View(model);
    }

    [HttpGet]
    [Route("/manager/club")]
    [Authorize(Policy = Permissions.ClubsAdd)]
    public async Task<IActionResult> Add()
    {
        var model = await clubsService.CreateClubEditModelAsync(loggedInUserId);
        return View("Edit", model);
    }

    [HttpGet]
    [Route("/manager/club/{id:Guid}")]
    [Authorize(Policy = Permissions.ClubsEdit)]
    public async Task<IActionResult> EditAsync(Guid id)
    {
        var model = await clubsService.GetClubEditModelByIdAsync(id, loggedInUserId);
        if (model == null)
        {
            ErrorMessage(heroesCupLocalizer.Club["The club could not be found."], false);
            return RedirectToAction("List");
        }

        return View(model);
    }

    [HttpPost]
    [Route("/manager/club/save")]
    [Authorize(Policy = Permissions.ClubsSave)]
    public async Task<IActionResult> SaveAsync(ClubEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage(heroesCupLocalizer.General["ValidationModalTitle"]);
            return View("Edit", model);
        }

        try
        {
            var clubId = await clubsService.SaveClubEditModelAsync(model);
            if (clubId != Guid.Empty)
            {
                SuccessMessage(heroesCupLocalizer.Club["The club has been saved."]);
                return RedirectToAction("Edit", new { id = clubId });
            }
        }
        catch (Exception)
        {
            ErrorMessage(heroesCupLocalizer.General["Sorry, an error occurred while executing your request."]);
            return View("Edit", model);
        }

        ErrorMessage(heroesCupLocalizer.Club["The club could not be saved."], false);
        return View("Edit", model);
    }

    [HttpGet]
    [Route("/manager/club/delete")]
    [Authorize(Policy = Permissions.ClubsDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await clubsService.DeleteAsync(id);
        if (!result)
        {
            ErrorMessage(heroesCupLocalizer.Club["The club could not be deleted."], false);
            return RedirectToAction("List");
        }

        SuccessMessage(heroesCupLocalizer.Club["The club has been deleted."]);
        return RedirectToAction("List");
    }
}