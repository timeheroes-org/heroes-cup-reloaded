using HeroesCup.Data.Models;

namespace HeroesCup.Web.Services
{
    public interface IMissionContentsService
    {
        Task<MissionContent> GetMissionContentByMissionId(Guid missionId);

        Task SaveOrUpdateMissionContent(MissionContent missionContent, Mission mission, bool commit = false);
    }
}