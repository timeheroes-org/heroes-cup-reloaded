using HeroesCup.Localization;
using HeroesCup.Web.ClubsModule.Exceptions;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Controllers;

namespace HeroesCup.Web.ClubsModule.Controllers
{
    public class MissionIdeasController : ManagerController
    {
        private readonly IMissionIdeasService _missionIdeasService;
        private readonly ManagerLocalizer _heroesCupLocalizer;

        public MissionIdeasController(IMissionIdeasService missionIdeasService, ManagerLocalizer heroesCupLocalizer)
        {
            this._missionIdeasService = missionIdeasService;
            this._heroesCupLocalizer = heroesCupLocalizer;
        }

        [HttpGet]
        [Route("/manager/missionideas")]
        [Authorize(Policy = Permissions.MissionIdeas)]
        public async Task<IActionResult> ListAsync()
        {
            var model = await this._missionIdeasService.GetMissionIdeasListModelAsync();
            return View(model);
        }

        [HttpGet]
        [Route("/manager/missionidea")]
        [Authorize(Policy = Permissions.MissionIdeasAdd)]
        public IActionResult Add()
        {
            var model = this._missionIdeasService.CreateMissionIdeaEditModel();
            return View("Edit", model);
        }

        [HttpGet]
        [Route("/manager/missionidea/{id:Guid}")]
        [Authorize(Policy = Permissions.MissionIdeasEdit)]
        public async Task<IActionResult> EditAsync(Guid id)
        {
            var model = await this._missionIdeasService.GetMissionIdeaEditModelByIdAsync(id);
            if (model == null)
            {
                ErrorMessage(this._heroesCupLocalizer.MissionIdea["The mission idea could not be found."], false);
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
                ErrorMessage(this._heroesCupLocalizer.General["ValidationModalTitle"]);
                return View("Edit", model);
            }            

            try
            {
                var missionId = await this._missionIdeasService.SaveMissionIdeaEditModelAsync(model);
                if (missionId != Guid.Empty)
                {
                    SuccessMessage(this._heroesCupLocalizer.MissionIdea["The mission idea has been saved."]);
                    return RedirectToAction("Edit", new { id = missionId });
                }
            }
            catch (ExistingItemException)
            {
                ErrorMessage(this._heroesCupLocalizer.MissionIdea["There is already a mission idea with the same title."]);
                return View("Edit", model);
            } 
            catch (Exception)
            {
                ErrorMessage(this._heroesCupLocalizer.General["Sorry, an error occurred while executing your request."]);
                return View("Edit", model);
            }

            ErrorMessage(this._heroesCupLocalizer.MissionIdea["The mission idea could not be saved."], false);
            return View("Edit", model);
        }

        [HttpGet]
        [Route("/manager/missionidea/delete")]
        [Authorize(Policy = Permissions.MissionIdeasDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await this._missionIdeasService.DeleteMissionIdeaAsync(id);
            if (!result)
            {
                ErrorMessage(this._heroesCupLocalizer.MissionIdea["The mission idea could not be deleted."], false);
                return RedirectToAction("List");
            }

            SuccessMessage(this._heroesCupLocalizer.MissionIdea["The mission idea has been deleted."]);
            return RedirectToAction("List");
        }

        [HttpPost]
        [Route("/manager/missionidea/publish")]
        [Authorize(Policy = Permissions.MissionIdeasPublish)]
        public async Task<IActionResult> PublishAsync(MissionIdeaEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            var result = await this._missionIdeasService.PublishMissionIdeaAsync(model.MissionIdea.Id);
            if (result)
            {
                SuccessMessage(this._heroesCupLocalizer.MissionIdea["The mission idea has been published."]);
                return RedirectToAction("Edit", new { id = model.MissionIdea.Id });
            }

            ErrorMessage(this._heroesCupLocalizer.MissionIdea["The mission idea could not be published."], false);
            return View("Edit", model);
        }

        [HttpPost]
        [Route("/manager/missionidea/unpublish")]
        [Authorize(Policy = Permissions.MissionIdeasPublish)]
        public async Task<IActionResult> UnpublishAsync(MissionIdeaEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            var result = await this._missionIdeasService.UnpublishMissionIdeaAsync(model.MissionIdea.Id);
            if (result)
            {
                SuccessMessage(this._heroesCupLocalizer.MissionIdea["The mission idea has been unpublished."]);
                return RedirectToAction("Edit", new { id = model.MissionIdea.Id });
            }

            ErrorMessage(this._heroesCupLocalizer.MissionIdea["The mission idea could not be unpublished."], false);
            return View("Edit", model);
        }
    }
}