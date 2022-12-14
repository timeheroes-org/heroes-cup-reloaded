using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.Common;
using HeroesCup.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroesCup.Web.Services;

public class ClubsService : IClubsService
{
    private readonly HeroesCupDbContext _dbContext;
    private readonly IHeroesService _heroesService;
    private readonly IImagesService _imagesService;

    private readonly string _dateTimeFormat;

    public ClubsService(HeroesCupDbContext dbContext, IImagesService imagesService, IHeroesService heroesService,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _imagesService = imagesService;
        _heroesService = heroesService;
        _dateTimeFormat = configuration["DateТimeFormat"];
    }

    public Task<IEnumerable<String>> GetSchools()
    {
        return  Task.FromResult<IEnumerable<string>>(_dbContext.Clubs.Select(c => c.OrganizationName).Distinct());
    }
    public async Task<ClubEditModel> CreateClubEditModelAsync(Guid? ownerId)
    {
        List<Mission> missions;
        List<Hero> heroes;
        if (ownerId.HasValue)
        {
            missions = await _dbContext.Missions.Where(m => m.OwnerId == ownerId.Value).ToListAsync();
            heroes = await _dbContext.Heroes.Where(h => h.Club.OwnerId == ownerId.Value).ToListAsync();
        }
        else
        {
            missions = await _dbContext.Missions.ToListAsync();
            heroes = await _dbContext.Heroes.ToListAsync();
        }

        var model = new ClubEditModel
        {
            Club = new Club(),
            Missions = missions != null ? missions : new List<Mission>(),
            Heroes = heroes != null ? heroes : new List<Hero>()
        };

        model.Club.OwnerId = ownerId.HasValue ? ownerId.Value : Guid.Empty;
        return model;
    }

    public async Task<ClubEditModel> GetClubEditModelByIdAsync(Guid id, Guid? ownerId)
    {
        var club = await _dbContext.Clubs
            .Include(c => c.ClubImages)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (club == null) return null;

        if (ownerId.HasValue && club.OwnerId != ownerId) return null;

        var missions = club.Missions;

        var model = await CreateClubEditModelAsync(ownerId);
        model.Club = club;
        var coordinators = await GetClubCoordinatorsAsync(club.Id);
        if (coordinators != null)
        {
            model.Coordinators = coordinators;
            model.CoordinatorsIds = coordinators.Select(c => c.Id).ToArray();
        }

        if (club.ClubImages != null && club.ClubImages.Count > 0)
        {
            var clubImage = _imagesService.GetClubImage(club.Id);
            model.ClubImage = Guid.Empty != clubImage.ImageId ? string.Concat(clubImage.Image.Id.ToString(),"/",clubImage.Image.Filename) : null;
        }

        model.Missions = club.Missions;
        model.Heroes = club.Heroes;

        return model;
    }

    public async Task<ClubListModel> GetClubListModelAsync(Guid? ownerId)
    {
        var clubs = new List<Club>();
        clubs = await _dbContext.Clubs
            .Include(c => c.Heroes)
            .OrderByDescending(c => c.UpdatedOn)
            .ToListAsync();

        if (ownerId.HasValue) clubs = clubs.Where(c => c.OwnerId == ownerId.Value).ToList();

        var model = new ClubListModel
        {
            Clubs = clubs.Select(c => new ClubListItem
            {
                Id = c.Id,
                Name = c.Name,
                OrganizationType = c.OrganizationType,
                OrganizationName = c.OrganizationName,
                OrganizationNumber = c.OrganizationNumber,
                HeroesCount = c.Heroes != null ? c.Heroes.Count() : 0,
                LastUpdateOn = c.UpdatedOn.ToUniversalDateTime().ToLocalTime().ToString(_dateTimeFormat)
            })
        };

        return model;
    }

    public async Task<Guid> SaveClubEditModelAsync(ClubEditModel model)
    {
        var club = await _dbContext.Clubs
            .Include(c => c.ClubImages)
            .Include(c => c.Heroes)
            .Include(c => c.Missions)
            .FirstOrDefaultAsync(h => h.Id == model.Club.Id && h.OwnerId == model.Club.OwnerId);

        if (club == null)
        {
            club = new Club();
            club.Id = model.Club.Id != Guid.Empty ? model.Club.Id : Guid.NewGuid();
            club.OwnerId = model.Club.OwnerId;
            club.CreatedOn = DateTime.Now.ToUnixMilliseconds();
            _dbContext.Clubs.Add(club);
        }

        club.Name = model.Club.Name.TrimInput();
        club.Location = model.Club.Location;
        club.OrganizationType = model.Club.OrganizationType;
        club.OrganizationName = model.Club.OrganizationName.TrimInput();
        club.OrganizationNumber = model.Club.OrganizationNumber.TrimInput();
        club.Description = model.Club.Description;


        // set club's heroes
        if (model.HeroesIds != null && model.HeroesIds.Any())
        {
            club.Heroes = new List<Hero>();
            foreach (var heroId in model.HeroesIds)
            {
                var hero = _dbContext.Heroes.FirstOrDefault(h => h.Id == heroId);
                club.Heroes.Add(hero);
            }
        }

        // set club coordinators
        if (model.CoordinatorsIds != null && model.CoordinatorsIds.All(c => c != Guid.Empty))
            await _heroesService.SaveCoordinatorsAsync(model.CoordinatorsIds, club, false);

        // set club's missions
        if (model.MissionsIds != null && model.MissionsIds.Any())
        {
            club.Missions = new List<Mission>();
            foreach (var missionId in model.MissionsIds)
            {
                var mission = _dbContext.Missions.FirstOrDefault(h => h.Id == missionId);
                club.Missions.Add(mission);
            }
        }

        // set club logo
        if (model.UploadedLogo != null)
        {
            var image = _imagesService.MapFormFileToImage(model.UploadedLogo);
            await _imagesService.CreateClubImageAsync(image, club);
        }

        club.UpdatedOn = DateTime.Now.ToUnixMilliseconds();

        await _dbContext.SaveChangesAsync();
        return club.Id;
    }

    public async Task<IEnumerable<Hero>> GetClubCoordinatorsAsync(Guid clubId)
    {
        IEnumerable<Hero> coordinators = null;
        var club = await _dbContext.Clubs.FirstOrDefaultAsync(c => c.Id == clubId);
        if (club == null) return null;

        if (club.Heroes != null && club.Heroes.Count > 0) coordinators = club.Heroes.Where(c => c.IsCoordinator);

        return coordinators;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var club = _dbContext.Clubs.FirstOrDefault(c => c.Id == id);
        if (club == null) return false;

        _dbContext.Clubs.Remove(club);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public IEnumerable<Club> GetAllClubs()
    {
        return _dbContext.Clubs
            .Include(c => c.Heroes);
    }

    public async Task<List<Club>> GetAllClubsWithImages()
    {
        return await _dbContext.Clubs
            .Include(c => c.ClubImages)
            .ThenInclude(c=>c.Image).ToListAsync();
    }
}