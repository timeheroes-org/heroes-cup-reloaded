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
            var isCoordinatorPermission = permission.Name == ClubsModule.Permissions.Clubs ||
                                          permission.Name == ClubsModule.Permissions.ClubsAdd ||
                                          permission.Name == ClubsModule.Permissions.ClubsDelete ||
                                          permission.Name == ClubsModule.Permissions.ClubsEdit ||
                                          permission.Name == ClubsModule.Permissions.ClubsSave ||
                                          permission.Name == ClubsModule.Permissions.Heroes ||
                                          permission.Name == ClubsModule.Permissions.HeroesAdd ||
                                          permission.Name == ClubsModule.Permissions.HeroesDelete ||
                                          permission.Name == ClubsModule.Permissions.HeroesEdit ||
                                          permission.Name == ClubsModule.Permissions.HeroesSave ||
                                          permission.Name == ClubsModule.Permissions.HeroesAddCoordinator ||
                                          permission.Name == ClubsModule.Permissions.Missions ||
                                          permission.Name == ClubsModule.Permissions.MissionsAdd ||
                                          permission.Name == ClubsModule.Permissions.MissionsDelete ||
                                          permission.Name == ClubsModule.Permissions.MissionsEdit ||
                                          permission.Name == ClubsModule.Permissions.MissionsSave ||
                                          permission.Name == ClubsModule.Permissions.Stories ||
                                          permission.Name == ClubsModule.Permissions.StoriesAdd ||
                                          permission.Name == ClubsModule.Permissions.StoriesDelete ||
                                          permission.Name == ClubsModule.Permissions.StoriesEdit ||
                                          permission.Name == ClubsModule.Permissions.StoriesSave ||
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