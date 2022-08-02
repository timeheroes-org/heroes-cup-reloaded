using HeroesCup.Data.Models;
using System;
using System.Threading.Tasks;

namespace HeroesCup.Web.Services
{
    public interface IMissionContentsService
    {
        Task<MissionContent> GetMissionContentByMissionId(Guid missionId);

        Task SaveOrUpdateMissionContent(MissionContent missionContent, Mission mission, bool commit = false);
    }
}
