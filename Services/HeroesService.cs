using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Models;
using HeroesCup.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HeroesCup.Web.Services
{
    public class HeroesService : IHeroesService
    {
        private readonly HeroesCupDbContext _dbContext;

        public HeroesService(HeroesCupDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<HeroListModel> GetHeroListModelAsync(Guid? ownerId)
        {
            var heroes = new List<Hero>();
            heroes = await this._dbContext.Heroes
                .Include(h => h.Club)
                .ToListAsync();

            if (ownerId.HasValue)
            {
                heroes = heroes.Where(h => h.Club.OwnerId == ownerId.Value).ToList();
            }

            if (heroes == null)
            {
                heroes = new List<Hero>();
            }

            var model = new HeroListModel()
            {
                Heroes = heroes.OrderBy(h => h.Name)
                    .Select(m => new HeroListItem()
                    {
                        Id = m.Id,
                        Name = m.Name,
                        ClubId = m.ClubId,
                        ClubName = m.Club.Name
                    })

            };

            return model;
        }

        public async Task<HeroEditModel> CreateHeroEditModelAsync(Guid? ownerId)
        {
            var clubs = new List<Club>();
            var missions = new List<Mission>();
            if (ownerId.HasValue)
            {
                clubs = await this._dbContext.Clubs.Where(c => c.OwnerId == ownerId.Value).ToListAsync();
                missions = await this._dbContext.Missions.Where(m => m.OwnerId == ownerId.Value).ToListAsync();
            }
            else
            {
                clubs = await this._dbContext.Clubs.ToListAsync();
                missions = await this._dbContext.Missions.ToListAsync();
            }

            return new HeroEditModel()
            {
                Hero = new Hero(),
                Clubs = clubs,
                Missions = missions
            };
        }

        public async Task<HeroEditModel> GetHeroEditModelByIdAsync(Guid id, Guid? ownerId)
        {
            var hero = await this._dbContext.Heroes
                .Include(h => h.Club)
                .Include(x => x.HeroMissions)
                .ThenInclude(x => x.Mission)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hero == null)
            {
                return null;
            }

            if (ownerId.HasValue && hero.Club.OwnerId != ownerId)
            {
                return null;
            }

            var missions = hero.HeroMissions
                .Where(h => h.HeroId == id)
                .Select(x => x.Mission);

            IEnumerable<Club> clubs = null;
            if (ownerId.HasValue)
            {
                clubs = await this._dbContext.Clubs
                    .Where(c => c.OwnerId == ownerId.Value)
                    .ToListAsync();
            }
            else
            {
                clubs = await this._dbContext.Clubs.ToListAsync();
            }

            if (clubs == null)
            {
                clubs = new List<Club>();
            }

            var model = await CreateHeroEditModelAsync(ownerId);
            model.Hero = hero;
            model.Missions = missions;
            model.Clubs = clubs;

            return model;
        }

        public async Task<Guid> SaveHeroEditModelAsync(HeroEditModel model)
        {
            var hero = await this._dbContext.Heroes
                .Include(x => x.HeroMissions)
                .ThenInclude(x => x.Mission)
                .FirstOrDefaultAsync(h => h.Id == model.Hero.Id);

            if (hero == null)
            {
                hero = new Hero();
                hero.Id = model.Hero.Id != Guid.Empty ? model.Hero.Id : Guid.NewGuid();
                this._dbContext.Heroes.Add(hero);
            }

            hero.Name = model.Hero.Name;
            hero.ClubId = model.ClubId != Guid.Empty ? model.ClubId : model.Hero.Club.Id;

            if (model.Hero.IsCoordinator)
            {
                //var heroesFromClub = await this.dbContext.Heroes.Where(h => h.ClubId == hero.ClubId).ToListAsync();
                //foreach (var h in heroesFromClub)
                //{
                //    h.IsCoordinator = false;
                //}

                hero.IsCoordinator = model.Hero.IsCoordinator;
            }

            if (model.Missions != null && model.Missions.Any())
            {
                hero.HeroMissions = model.Missions.Select(m => new HeroMission()
                {
                    Mission = m,
                    MissionId = m.Id,
                    Hero = hero,
                    HeroId = hero.Id
                }).ToList();
            }

            await _dbContext.SaveChangesAsync();
            return hero.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var hero = this._dbContext.Heroes.FirstOrDefault(c => c.Id == id);
            if (hero == null)
            {
                return false;
            }

            this._dbContext.Heroes.Remove(hero);
            await this._dbContext.SaveChangesAsync();
            return true;
        }

        public async Task SaveCoordinatorsAsync(IEnumerable<Guid> newCoordinatorsIds, Club club, bool commit)
        {
            if (club == null)
            {
                throw new ArgumentNullException("Club cannot be null.");
            }

            foreach (var hero in club.Heroes)
            {
                hero.IsCoordinator = false;
            }

            newCoordinatorsIds.ToList().ForEach(id =>
            {
                var newCoordinator = this._dbContext.Heroes.FirstOrDefault(h => h.Id == id);
                newCoordinator.IsCoordinator = true;
            });

            if (commit)
            {
                await this._dbContext.SaveChangesAsync();
            }
        }


        public async Task<ICollection<Hero>> GetHeroes(Guid? clubId, Guid? ownerId)
        {
            var heroes = this._dbContext.Heroes
                .Include(h => h.Club)
                .Select(x => x);

            if (clubId.HasValue)
            {
                heroes = heroes.Where(h => h.ClubId == clubId);
            }

            if (ownerId.HasValue)
            {
                heroes = heroes.Where(h => h.Club.OwnerId == ownerId.Value);
            }

            return await heroes.OrderBy(h => h.Name).ToListAsync();
        }

        public async Task<Hero> GetHeroById(Guid id)
        {
            return await this._dbContext.Heroes.FirstOrDefaultAsync(h => h.Id == id);
        }
    }
}