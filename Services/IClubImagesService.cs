using HeroesCup.Data.Models;

namespace HeroesCup.Web.Services
{
    public interface IClubImagesService
    {
        Task CreateClubImageAsync(Image image, Club club);

        Task<ClubImage> GetClubImage(Guid clubId);

        Task DeleteClubImageAsync(ClubImage clubImage, bool commit = false);

        string getClubImageId(Guid clubId);
    }
}