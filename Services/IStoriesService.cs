using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Models;

namespace HeroesCup.Web.Services;

public interface IStoriesService
{
    Task<StoryListModel> GetStoryListModelAsync(Guid? ownerId);

    Task<StoryEditModel> CreateStoryEditModelAsync(Guid? ownerId);

    Task<Guid> SaveStoryEditModelAsync(StoryEditModel model);

    Task<bool> PublishStoryEditModelAsync(Guid storyId);

    Task<bool> UnpublishStoryEditModelAsync(Guid storyId);

    Task<StoryEditModel> GetStoryEditModelByIdAsync(Guid id, Guid? ownerId);

    Task<bool> DeleteAsync(Guid id);

    IEnumerable<Story> GetAllPublishedStories();

    Task<Story> GetStoryByMissionSlugAsync(string missionSlug);
}