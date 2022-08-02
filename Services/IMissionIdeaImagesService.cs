using HeroesCup.Data.Models;

namespace HeroesCup.Web.Services;

public interface IMissionIdeaImagesService
{
    Task CreateMissionIdeaImageAsync(Image image, MissionIdea missionIdea);

    Task DeleteMissionIdeaImageAsync(MissionIdeaImage missionIdeaImage, bool commit = false);

    Task<MissionIdeaImage> GetMissionIdeaImageAsync(Guid missionIdeaId);
}