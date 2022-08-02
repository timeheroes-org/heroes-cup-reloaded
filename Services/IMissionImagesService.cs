using HeroesCup.Data.Models;

namespace HeroesCup.Web.Services
{
    public interface IMissionImagesService
    {
        Task CreateMissionImageAsync(Image image, Mission mission);

        Task DeleteMissionImageAsync(MissionImage missionImage, bool commit = false);

        Task<MissionImage> GetMissionImage(Guid missionId);
    }
}