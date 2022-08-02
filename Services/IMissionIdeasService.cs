using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Data.Models;

namespace HeroesCup.Web.Services
{
    public interface IMissionIdeasService
    {
        Task<MissionIdeaListModel> GetMissionIdeasListModelAsync();

        IEnumerable<MissionIdea> GetAllPublishedMissionIdeas();

        MissionIdeaEditModel CreateMissionIdeaEditModel();

        Task<MissionIdeaEditModel> GetMissionIdeaEditModelByIdAsync(Guid id);

        Task<MissionIdeaEditModel> GetMissionIdeaEditModelBySlugAsync(string slug);

        Task<Guid> SaveMissionIdeaEditModelAsync(MissionIdeaEditModel model);

        Task<bool> DeleteMissionIdeaAsync(Guid id);

        Task<bool> PublishMissionIdeaAsync(Guid missionIdeaId);

        Task<bool> UnpublishMissionIdeaAsync(Guid missionIdeaId);
    }
}
