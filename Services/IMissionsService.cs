using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.Models;

namespace HeroesCup.Web.Services;

public interface IMissionsService
{
    IEnumerable<MissionIdeaViewModel> GetMissionIdeaViewModels();

    IEnumerable<MissionViewModel> GetMissionViewModels();

    Task<IEnumerable<MissionViewModel>> GetPinnedMissionViewModels();

    IDictionary<string, int> GetMissionsPerLocation();

    int GetAllMissionsCount();
    IQueryable<Story> GetAllPublishedStories();
    IEnumerable<MissionViewModel> GetMissionViewModelsByLocation(string location);

    Task<MissionViewModel> GetMissionViewModelBySlugAsync(string slug);

    Task<MissionIdeaViewModel> GetMissionIdeaViewModelBySlugAsync(string slug);

    IQueryable<StoryViewModel> GetAllPublishedStoryViewModels();

    Task<StoryViewModel> GetStoryViewModelByMissionSlugAsync(string missionSlug);

    string ParseLocation(string location);
    Task<MissionListModel> GetMissionListModelAsync(Guid? ownerId = null);

    Task<MissionEditModel> CreateMissionEditModelAsync(Guid? ownerId = null);

    Task<Guid> SaveMissionEditModelAsync(MissionEditModel model);

    Task<bool> PublishMissionEditModelAsync(Guid missionId);

    Task<bool> UnPublishMissionEditModelAsync(Guid missionId);

    Task<MissionEditModel> GetMissionEditModelByIdAsync(Guid id, Guid? ownerId = null);

    Task<MissionEditModel> GetMissionEditModelBySlugAsync(string slug);

    Task<bool> DeleteAsync(Guid id);

    TimeSpan GetMissionDuration(long startDate, long endDate);

    Task<IEnumerable<Mission>> GetMissionsBySchoolYear(string schoolYear);

    IEnumerable<string> GetMissionSchoolYears();

    IEnumerable<Mission> GetAllPublishedMissions();
    IEnumerable<Tuple<string, string>> GetMissionImagesIds(Guid missionId);
    Task<bool> PinMissionEditModelAsync(Guid id);

    Task<bool> UnpinMissionEditModelAsync(Guid id);

    Task<IEnumerable<Mission>> GetPinnedMissions();

    Task SaveMissionDurationHours(Mission mission, int durationHours, bool commit = false);

    Task SaveMissionHeroes(Mission mission, IEnumerable<Guid> ids, bool commit = false);
    Task<List<Mission>> GetAllPublishedMissionsWithContentAndImages();
}