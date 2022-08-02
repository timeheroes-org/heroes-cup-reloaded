using HeroesCup.Localization;
using HeroesCup.Web.Exceptions;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Controllers;

namespace HeroesCup.Web.ClubsModule.Controllers;

public class MissionIdeasController : ManagerController
{
    private readonly ManagerLocalizer _heroesCupLocalizer;
    private readonly IMissionIdeasService _missionIdeasService;

    public MissionIdeasController(IMissionIdeasService missionIdeasService, ManagerLocalizer heroesCupLocalizer)
    {
        _missionIdeasService = missionIdeasService;
        _heroesCupLocalizer = heroesCupLocalizer;
    }

    [HttpGet]
    [Route("/manager/missionideas")]
    [Authorize(Policy = Permissions.MissionIdeas)]
    public async Task<IActionResult> ListAsync()
    {
        var model = await _missionIdeasService.GetMissionIdeasListModelAsync();
        return View(model);
    }

    [HttpGet]
    [Route("/manager/missionidea")]
    [Authorize(Policy = Permissions.MissionIdeasAdd)]
    public IActionResult Add()
    {
        var model = _missionIdeasService.CreateMissionIdeaEditModel();
        return View("Edit", model);
    }

    [HttpGet]
    [Route("/manager/missionidea/{id:Guid}")]
    [Authorize(Policy = Permissions.MissionIdeasEdit)]
    public async Task<IActionResult> EditAsync(Guid id)
    {
        var model = await _missionIdeasService.GetMissionIdeaEditModelByIdAsync(id);
        if (model == null)
        {
            ErrorMessage(_heroesCupLocalizer.MissionIdea["The mission idea could not be found."], false);
            return RedirectToAction("List");
        }

        return View(model);
    }

    [HttpPost]
    [Route("/manager/missionidea/save")]
    [Authorize(Policy = Permissions.MissionIdeasSave)]
    public async Task<IActionResult> SaveAsync(MissionIdeaEditModel model)
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage(_heroesCupLocalizer.General["ValidationModalTitle"]);
            return View("Edit", model);
        }

        try
        {
            var missionId = await _missionIdeasService.SaveMissionIdeaEditModelAsync(model);
            if (missionId != Guid.Empty)
            {
                SuccessMessage(_heroesCupLocalizer.MissionIdea["The mission idea has been saved."]);
                return RedirectToAction("Edit", new { id = missionId });
            }
        }
        catch (ExistingItemException)
        {
            ErrorMessage(_heroesCupLocalizer.MissionIdea["There is already a mission idea with the same title."]);
            return View("Edit", model);
        }
        catch (Exception)
        {
            ErrorMessage(_heroesCupLocalizer.General["Sorry, an error occurred while executing your request."]);
            return View("Edit", model);
        }

        ErrorMessage(_heroesCupLocalizer.MissionIdea["The mission idea could not be saved."], false);
        return View("Edit", model);
    }

    [HttpGet]
    [Route("/manager/missionidea/delete")]
    [Authorize(Policy = Permissions.MissionIdeasDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _missionIdeasService.DeleteMissionIdeaAsync(id);
        if (!result)
        {
            ErrorMessage(_heroesCupLocalizer.MissionIdea["The mission idea could not be deleted."], false);
            return RedirectToAction("List");
        }

        SuccessMessage(_heroesCupLocalizer.MissionIdea["The mission idea has been deleted."]);
        return RedirectToAction("List");
    }

    [HttpPost]
    [Route("/manager/missionidea/publish")]
    [Authorize(Policy = Permissions.MissionIdeasPublish)]
    public async Task<IActionResult> PublishAsync(MissionIdeaEditModel model)
    {
        if (!ModelState.IsValid) return View("Edit", model);

        var result = await _missionIdeasService.PublishMissionIdeaAsync(model.MissionIdea.Id);
        if (result)
        {
            SuccessMessage(_heroesCupLocalizer.MissionIdea["The mission idea has been published."]);
            return RedirectToAction("Edit", new { id = model.MissionIdea.Id });
        }

        ErrorMessage(_heroesCupLocalizer.MissionIdea["The mission idea could not be published."], false);
        return View("Edit", model);
    }

    [HttpPost]
    [Route("/manager/missionidea/unpublish")]
    [Authorize(Policy = Permissions.MissionIdeasPublish)]
    public async Task<IActionResult> UnpublishAsync(MissionIdeaEditModel model)
    {
        if (!ModelState.IsValid) return View("Edit", model);

        var result = await _missionIdeasService.UnpublishMissionIdeaAsync(model.MissionIdea.Id);
        if (result)
        {
            SuccessMessage(_heroesCupLocalizer.MissionIdea["The mission idea has been unpublished."]);
            return RedirectToAction("Edit", new { id = model.MissionIdea.Id });
        }

        ErrorMessage(_heroesCupLocalizer.MissionIdea["The mission idea could not be unpublished."], false);
        return View("Edit", model);
    }
}