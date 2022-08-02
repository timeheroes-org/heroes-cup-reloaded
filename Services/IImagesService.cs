using HeroesCup.Data.Models;

namespace HeroesCup.Web.Services;

public interface IImagesService : IClubImagesService, IMissionImagesService, IStoryImageService,
    IMissionIdeaImagesService
{
    Task<Image> GetImage(Guid id);

    Task<Image> GetImageByFileName(string filename);

    string GetImageSource(string contentType, byte[] bytes);

    byte[] GetByteArrayFromImage(IFormFile file);

    string GetFilename(IFormFile file, Guid imageId);

    string GetFileContentType(IFormFile file);

    Image MapFormFileToImage(IFormFile file);

    string GetImageFilename(Image image);

    Task<string> GetImageFilename(Guid id);
}