using HeroesCup.Data.Models;

namespace HeroesCup.Web.Services;

public interface IImagesService : IClubImagesService, IMissionImagesService, IStoryImageService,
    IMissionIdeaImagesService
{
    Task<Image> GetImage(Guid id);


    Image MapFormFileToImage(IFormFile file);

}