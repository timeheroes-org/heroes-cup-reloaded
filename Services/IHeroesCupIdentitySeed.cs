namespace HeroesCup.Web.Services;

public interface IHeroesCupIdentitySeed
{
    Task SeedRolesAsync();
    Task SeedIdentityAsync();
}