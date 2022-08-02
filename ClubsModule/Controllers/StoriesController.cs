using HeroesCup.Localization;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.ClubsModule.Security;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Controllers;

namespace HeroesCup.Web.ClubsModule.Controllers;

public class StoriesController : ManagerController
{
    private readonly ManagerLocalizer heroesCupLocalizer;
    private readonly IStoriesService storiesService;
    private readonly IUserManager userManager;
    private readonly Guid? loggedInUserId;

    public StoriesController(IStoriesService stroiesService, IUserManager userManager,
        ManagerLocalizer heroesCupLocalizer)
    {
        storiesService = stroiesService;
        this.userManager = userManager;
        loggedInUserId = this.userManager.GetCurrentUserId();
        this.heroesCupLocalizer = heroesCupLocalizer;
    }

    [HttpGet]
    [Route("/manager/stories")]
    [Authorize(Policy = Permissions.Stories)]
    public async Task<IActionResult> ListAsync()
    {
        var model = await storiesService.GetStoryListModelAsync(loggedInUserId);
        return View(model);
    }

    [HttpGet]
    [Route("/manager/story")]
    [Authorize(Policy = Permissions.StoriesAdd)]
    public async Task<IActionResult> Add()
    {
        var model = await storiesService.CreateStoryEditModelAsync(loggedInUserId);
        return View("Edit", model);
    }

    [HttpPost]
    [Route("/manager/story/save")]
    [Authorize(Policy = Permissions.StoriesSave)]
    public async Task<IActionResult> SaveAsync(StoryEditModel model)
    {
        if (!ModelState.IsValid)
        {
            var validModel = await storiesService.GetStoryEditModelByIdAsync(model.Story.Id, loggedInUserId);
            ErrorMessage(heroesCupLocalizer.Story["The story could not be saved."]);
            return View("Edit", validModel);
        }

        try
        {
            var storyId = await storiesService.SaveStoryEditModelAsync(model);
            if (storyId != Guid.Empty)
            {
                SuccessMessage(heroesCupLocalizer.Story["The story has been saved."]);
                return RedirectToAction("Edit", new { id = storyId });
            }
        }
        catch (Exception)
        {
            ErrorMessage(heroesCupLocalizer.General["Sorry, an error occurred while executing your request."]);
            return View("Edit", model);
        }

        ErrorMessage(heroesCupLocalizer.Story["The story could not be saved."]);
        return RedirectToAction("Edit", new { id = model.Story.Id });
    }

    [HttpGet]
    [Route("/manager/story/{id:Guid}")]
    [Authorize(Policy = Permissions.StoriesEdit)]
    public async Task<IActionResult> EditAsync(Guid id)
    {
        var model = await storiesService.GetStoryEditModelByIdAsync(id, loggedInUserId);
        if (model == null)
        {
            ErrorMessage(heroesCupLocalizer.Story["The story could not be found."], false);
            return RedirectToAction("List");
        }

        return View(model);
    }

    [HttpGet]
    [Route("/manager/story/delete")]
    [Authorize(Policy = Permissions.StoriesDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await storiesService.DeleteAsync(id);
        if (!result)
        {
            ErrorMessage(heroesCupLocalizer.Story["The story could not be deleted."], false);
            return RedirectToAction("List");
        }

        SuccessMessage(heroesCupLocalizer.Story["The story has been deleted."]);
        return RedirectToAction("List");
    }

    [HttpPost]
    [Route("/manager/story/publish")]
    [Authorize(Policy = Permissions.StoriesPublish)]
    public async Task<IActionResult> PublishAsync(StoryEditModel model)
    {
        if (!ModelState.IsValid) return View("Edit", model);

        var result = await storiesService.PublishStoryEditModelAsync(model.Story.Id);
        if (result)
        {
            SuccessMessage(heroesCupLocalizer.Story["The story has been published."]);
            return RedirectToAction("Edit", new { id = model.Story.Id });
        }

        ErrorMessage(heroesCupLocalizer.Story["The story could not be published."], false);
        return View("Edit", model);
    }

    [HttpPost]
    [Route("/manager/story/unpublish")]
    [Authorize(Policy = Permissions.StoriesPublish)]
    public async Task<IActionResult> UnpublishAsync(StoryEditModel model)
    {
        if (!ModelState.IsValid) return View("Edit", model);

        var result = await storiesService.UnpublishStoryEditModelAsync(model.Story.Id);
        if (result)
        {
            SuccessMessage(heroesCupLocalizer.Story["The story has been unpublished."]);
            return RedirectToAction("Edit", new { id = model.Story.Id });
        }

        ErrorMessage(heroesCupLocalizer.Story["The story could not be unpublished."], false);
        return View("Edit", model);
    }
}