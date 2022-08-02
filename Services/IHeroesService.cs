﻿using HeroesCup.Data.Models;
using HeroesCup.Web.ClubsModule.Models;

namespace HeroesCup.Web.Services 
{
    public interface IHeroesService
    {
        Task<HeroListModel> GetHeroListModelAsync(Guid? ownerId);

        Task<HeroEditModel> CreateHeroEditModelAsync(Guid? ownerId);

        Task<HeroEditModel> GetHeroEditModelByIdAsync(Guid id, Guid? ownerId);

        Task<Guid> SaveHeroEditModelAsync(HeroEditModel model);

        Task<bool> DeleteAsync(Guid id);

        Task SaveCoordinatorsAsync(IEnumerable<Guid> newCoordinatorsIds, Club club, bool commit);

        Task<ICollection<Hero>> GetHeroes(Guid? clubId, Guid? ownerId);

        Task<Hero> GetHeroById(Guid id);
    }
}