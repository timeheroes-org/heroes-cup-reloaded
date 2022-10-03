using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Models;

namespace HeroesCup.Web.Services;

public interface IClubsService
{
    public Task<IEnumerable<String>> GetSchools();
    Task<ClubListModel> GetClubListModelAsync(Guid? ownerId);

    Task<ClubEditModel> CreateClubEditModelAsync(Guid? ownerId);

    Task<ClubEditModel> GetClubEditModelByIdAsync(Guid id, Guid? ownerId);

    Task<Guid> SaveClubEditModelAsync(ClubEditModel model);

    Task<IEnumerable<Hero>> GetClubCoordinatorsAsync(Guid clubId);

    Task<bool> DeleteAsync(Guid id);

    IEnumerable<Club> GetAllClubs();
    Task<Task<List<Club>>> GetAllClubsWithImages();
}