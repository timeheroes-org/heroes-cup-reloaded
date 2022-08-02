using HeroesCup.Data.Models;
using HeroesCup.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HeroesCup.Web.Controllers;

public class ImgController : Controller
{
    private readonly IImagesService _imagesService;

    public ImgController(IImagesService imagesService)
    {
        _imagesService = imagesService;
    }

    [Route("img/{filenameOrId}")]
    [HttpGet]
    public async Task<IActionResult> GetImageByFileName(string filenameOrId)
    {
        if (filenameOrId == null) return NotFound();

        Guid imageId;
        var isValidId = Guid.TryParse(filenameOrId, out imageId);
        Image image = null;

        if (!isValidId)
            image = await _imagesService.GetImageByFileName(filenameOrId);
        else
            image = await _imagesService.GetImage(imageId);

        if (image == null) return NotFound();

        Response.Headers.Add("Cache-Control", "max-age=31536000");
        return File(image.Bytes, image.ContentType);
    }
}