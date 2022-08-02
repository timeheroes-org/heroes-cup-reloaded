using Microsoft.AspNetCore.Identity;
using Piranha;
using Piranha.AspNetCore.Identity.Data;
using Piranha.Security;
using IDb = Piranha.AspNetCore.Identity.IDb;

namespace HeroesCup.Web.Services;

public class IdentitySeed : IHeroesCupIdentitySeed
{
    private readonly IDb _db;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;


    public IdentitySeed(IDb db, UserManager<User> userManager, IConfiguration configuration)
    {
        this._db = db;
        this._userManager = userManager;
        this._configuration = configuration;
    }

    public async Task SeedIdentityAsync()
    {
        await SeedRolesAsync();
        var username = this._configuration["Identity:Users:Timeheroes:Name"];
        var email = this._configuration["Identity:Users:Timeheroes:Email"];
        if (!this._db.Users.Any(u => u.Email == email && u.UserName == username))
        {
            var user = new User
            {
                UserName = username,
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var createResult = await this._userManager.CreateAsync(user, "password");

            if (createResult.Succeeded)
            {
                var timeheroesRoleName = this._configuration["Identity:Roles:TimeheroesRole"];
                await this._userManager.AddToRoleAsync(user, timeheroesRoleName);
            }
        }

        await this._db.SaveChangesAsync();
    }

    public async Task SeedRolesAsync()
    {
        await SeedTimeheroesRoleAsync();
        await SeedCoordinatorRoleAsync();
    }

    private async Task SeedTimeheroesRoleAsync()
    {
        var timeheroesRoleName = this._configuration["Identity:Roles:TimeheroesRole"];
        if (!this._db.Roles.Any(r => r.Name == timeheroesRoleName || r.NormalizedName == timeheroesRoleName.ToUpper()))
        {
            var timeheroesRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = timeheroesRoleName,
                NormalizedName = timeheroesRoleName.ToUpper()
            };

            this._db.Roles.Add(timeheroesRole);
            AddPermissions(timeheroesRole, GetTimeheroesPermissions());
        }


        await this._db.SaveChangesAsync();
    }

    private async Task SeedCoordinatorRoleAsync()
    {
        var coordinatorRoleName = this._configuration["Identity:Roles:CoordinatorRole"];
        if (!this._db.Roles.Any(r => r.Name == coordinatorRoleName || r.NormalizedName == coordinatorRoleName.ToUpper()))
        {
            var coordinatorRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = coordinatorRoleName,
                NormalizedName = coordinatorRoleName.ToUpper()
            };

            this._db.Roles.Add(coordinatorRole);
            AddPermissions(coordinatorRole, GetCoordinatorPermissions());
        }

        await this._db.SaveChangesAsync();
    }

    private void AddPermissions(Role role, IEnumerable<PermissionItem> permissions)
    {
        foreach (var permission in permissions)
        {
            var roleClaim = this._db.RoleClaims.FirstOrDefault(c =>
                c.RoleId == role.Id && c.ClaimType == permission.Name && c.ClaimValue == permission.Name);
            if (roleClaim == null)
            {
                this._db.RoleClaims.Add(new IdentityRoleClaim<Guid>
                {
                    RoleId = role.Id,
                    ClaimType = permission.Name,
                    ClaimValue = permission.Name
                });
            }
        }
    }

    private IEnumerable<PermissionItem> GetCoordinatorPermissions()
    {
        var coordinatorPermissions = new HashSet<PermissionItem>();

        foreach (var permission in App.Permissions.GetPermissions())
        {
            var isCoordinatorPermission = permission.Name == HeroesCup.Web.ClubsModule.Permissions.Clubs ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.ClubsAdd ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.ClubsDelete ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.ClubsEdit ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.ClubsSave ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.Heroes ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.HeroesAdd ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.HeroesDelete ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.HeroesEdit ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.HeroesSave ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.HeroesAddCoordinator ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.Missions ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.MissionsAdd ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.MissionsDelete ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.MissionsEdit ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.MissionsSave ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.Stories ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.StoriesAdd ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.StoriesDelete ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.StoriesEdit ||
                                          permission.Name == HeroesCup.Web.ClubsModule.Permissions.StoriesSave ||
                                          permission.Name == Piranha.Manager.Permission.Admin;

            if (isCoordinatorPermission)
            {
                coordinatorPermissions.Add(permission);
            }
        }

        return coordinatorPermissions;
    }

    private IEnumerable<PermissionItem> GetTimeheroesPermissions()
    {
        return App.Permissions.GetPermissions();
    }
}