using HeroesCup.Localization;
using HeroesCup.Web.ClubsModule.Exceptions;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.ClubsModule.Security;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Controllers;

namespace HeroesCup.Web.ClubsModule.Controllers;

public class MissionsController : ManagerController
{
    private readonly ManagerLocalizer heroesCupLocalizer;
    private readonly IMissionsService missionsService;
    private readonly IUserManager userManager;
    private readonly Guid? loggedInUserId;

    public MissionsController(IMissionsService missionsService, IUserManager userManager,
        ManagerLocalizer heroesCupLocalizer)
    {
        this.missionsService = missionsService;
        this.userManager = userManager;
        loggedInUserId = this.userManager.GetCurrentUserId();
        this.heroesCupLocalizer = heroesCupLocalizer;
    }

    [HttpGet]
    [Route("/manager/missions")]
    [Authorize(Policy = Permissions.Missions)]
    public async Task<IActionResult> ListAsync()
    {
        var model = await missionsService.GetMissionListModelAsync(loggedInUserId);
        return View(model);
    }

    [HttpGet]
    [Route("/manager/mission")]
    [Authorize(Policy = Permissions.MissionsAdd)]
    public async Task<IActionResult> Add()
    {
        var model = await missionsService.CreateMissionEditModelAsync(loggedInUserId);
        return View("Edit", model);
    }

    [HttpPost]
    [Route("/manager/mission/save")]
    [Authorize(Policy = Permissions.MissionsSave)]
    public async Task<IActionResult> SaveAsync(MissionEditModel model)
    {
        if (model.Mission.ClubId == Guid.Empty && model.Clubs == null)
        {
            var newMissionModel = await missionsService.CreateMissionEditModelAsync();
            model.Clubs = newMissionModel.Clubs;
        }

        if (!ModelState.IsValid)
        {
            ErrorMessage(heroesCupLocalizer.General["ValidationModalTitle"]);
            return View("Edit", model);
        }

        try
        {
            var missionId = await missionsService.SaveMissionEditModelAsync(model);
            if (missionId != Guid.Empty)
            {
                SuccessMessage(heroesCupLocalizer.Mission["The mission has been saved."]);
                return RedirectToAction("Edit", new { id = missionId });
            }
        }
        catch (ExistingItemException)
        {
            ErrorMessage(heroesCupLocalizer.Mission["There is already a mission with the same title."]);
            return View("Edit", model);
        }
        catch (Exception)
        {
            ErrorMessage(heroesCupLocalizer.General["Sorry, an error occurred while executing your request."]);
            return View("Edit", model);
        }

        ErrorMessage(heroesCupLocalizer.Mission["The mission could not be saved."]);
        return View("Edit", model);
    }

    [HttpPost]
    [Route("/manager/mission/publish")]
    [Authorize(Policy = Permissions.MissionsPublish)]
    public async Task<IActionResult> PublishAsync(MissionEditModel model)
    {
        if (!ModelState.IsValid) return View("Edit", model);

        var result = await missionsService.PublishMissionEditModelAsync(model.Mission.Id);
        if (result)
        {
            SuccessMessage(heroesCupLocalizer.Mission["The mission has been published."]);
            return RedirectToAction("Edit", new { id = model.Mission.Id });
        }

        ErrorMessage(heroesCupLocalizer.Mission["The mission could not be published."], false);
        return View("Edit", model);
    }

    [HttpPost]
    [Route("/manager/mission/unpublish")]
    [Authorize(Policy = Permissions.MissionsPublish)]
    public async Task<IActionResult> UnpublishAsync(MissionEditModel model)
    {
        if (!ModelState.IsValid) return View("Edit", model);

        var result = await missionsService.UnPublishMissionEditModelAsync(model.Mission.Id);
        if (result)
        {
            SuccessMessage(heroesCupLocalizer.Mission["The mission has been unpublished."]);
            return RedirectToAction("Edit", new { id = model.Mission.Id });
        }

        ErrorMessage(heroesCupLocalizer.Mission["The mission could not be unpublished."], false);
        return View("Edit", model);
    }

    [HttpGet]
    [Route("/manager/mission/{id:Guid}")]
    [Authorize(Policy = Permissions.MissionsEdit)]
    public async Task<IActionResult> EditAsync(Guid id)
    {
        var model = await missionsService.GetMissionEditModelByIdAsync(id, loggedInUserId);
        if (model == null)
        {
            ErrorMessage(heroesCupLocalizer.Mission["The mission could not be found."], false);
            return RedirectToAction("List");
        }

        return View(model);
    }

    [HttpGet]
    [Route("/manager/mission/delete")]
    [Authorize(Policy = Permissions.MissionsDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await missionsService.DeleteAsync(id);
        if (!result)
        {
            ErrorMessage(heroesCupLocalizer.Mission["The mission could not be deleted."], false);
            return RedirectToAction("List");
        }

        SuccessMessage(heroesCupLocalizer.Mission["The mission has been deleted."]);
        return RedirectToAction("List");
    }

    [HttpPost]
    [Route("/manager/mission/pin")]
    [Authorize(Policy = Permissions.MissionsPublish)]
    public async Task<IActionResult> PinAsync(MissionEditModel model)
    {
        if (!ModelState.IsValid) return View("Edit", model);

        var result = await missionsService.PinMissionEditModelAsync(model.Mission.Id);
        if (result)
        {
            SuccessMessage(heroesCupLocalizer.Mission["The mission has been pinned to home page."]);
            return RedirectToAction("Edit", new { id = model.Mission.Id });
        }

        ErrorMessage(heroesCupLocalizer.Mission["The mission could not be pinned to home page."], false);
        return View("Edit", model);
    }

    [HttpPost]
    [Route("/manager/mission/unpin")]
    [Authorize(Policy = Permissions.MissionsPublish)]
    public async Task<IActionResult> UnpinAsync(MissionEditModel model)
    {
        if (!ModelState.IsValid) return View("Edit", model);

        var result = await missionsService.UnpinMissionEditModelAsync(model.Mission.Id);
        if (result)
        {
            SuccessMessage(heroesCupLocalizer.Mission["The mission has been unpinned from home page."]);
            return RedirectToAction("Edit", new { id = model.Mission.Id });
        }

        ErrorMessage(heroesCupLocalizer.Mission["The mission could not be unpinned from home page."], false);
        return View("Edit", model);
    }
}