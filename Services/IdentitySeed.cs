using HeroesCup.Web.ClubsModule;
using Microsoft.AspNetCore.Identity;
using Piranha;
using Piranha.AspNetCore.Identity.Data;
using Piranha.Security;
using IDb = Piranha.AspNetCore.Identity.IDb;
using Permission = Piranha.Manager.Permission;

namespace HeroesCup.Web.Services;

public class IdentitySeed : IHeroesCupIdentitySeed
{
    private readonly IConfiguration _configuration;
    private readonly IDb _db;
    private readonly UserManager<User> _userManager;


    public IdentitySeed(IDb db, UserManager<User> userManager, IConfiguration configuration)
    {
        _db = db;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task SeedIdentityAsync()
    {
        await SeedRolesAsync();
        var username = _configuration["Identity:Users:Timeheroes:Name"];
        var email = _configuration["Identity:Users:Timeheroes:Email"];
        if (!_db.Users.Any(u => u.Email == email && u.UserName == username))
        {
            var user = new User
            {
                UserName = username,
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var createResult = await _userManager.CreateAsync(user, "password");

            if (createResult.Succeeded)
            {
                var timeheroesRoleName = _configuration["Identity:Roles:TimeheroesRole"];
                await _userManager.AddToRoleAsync(user, timeheroesRoleName);
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task SeedRolesAsync()
    {
        await SeedTimeheroesRoleAsync();
        await SeedCoordinatorRoleAsync();
    }

    private async Task SeedTimeheroesRoleAsync()
    {
        var timeheroesRoleName = _configuration["Identity:Roles:TimeheroesRole"];
        if (!_db.Roles.Any(r => r.Name == timeheroesRoleName || r.NormalizedName == timeheroesRoleName.ToUpper()))
        {
            var timeheroesRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = timeheroesRoleName,
                NormalizedName = timeheroesRoleName.ToUpper()
            };

            _db.Roles.Add(timeheroesRole);
            AddPermissions(timeheroesRole, GetTimeheroesPermissions());
        }


        await _db.SaveChangesAsync();
    }

    private async Task SeedCoordinatorRoleAsync()
    {
        var coordinatorRoleName = _configuration["Identity:Roles:CoordinatorRole"];
        if (!_db.Roles.Any(r => r.Name == coordinatorRoleName || r.NormalizedName == coordinatorRoleName.ToUpper()))
        {
            var coordinatorRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = coordinatorRoleName,
                NormalizedName = coordinatorRoleName.ToUpper()
            };

            _db.Roles.Add(coordinatorRole);
            AddPermissions(coordinatorRole, GetCoordinatorPermissions());
        }

        await _db.SaveChangesAsync();
    }

    private void AddPermissions(Role role, IEnumerable<PermissionItem> permissions)
    {
        foreach (var permission in permissions)
        {
            var roleClaim = _db.RoleClaims.FirstOrDefault(c =>
                c.RoleId == role.Id && c.ClaimType == permission.Name && c.ClaimValue == permission.Name);
            if (roleClaim == null)
                _db.RoleClaims.Add(new IdentityRoleClaim<Guid>
                {
                    RoleId = role.Id,
                    ClaimType = permission.Name,
                    ClaimValue = permission.Name
                });
        }
    }

    private IEnumerable<PermissionItem> GetCoordinatorPermissions()
    {
        var coordinatorPermissions = new HashSet<PermissionItem>();

        foreach (var permission in App.Permissions.GetPermissions())
        {
            var isCoordinatorPermission = permission.Name == Permissions.Clubs ||
                                          permission.Name == Permissions.ClubsAdd ||
                                          permission.Name == Permissions.ClubsDelete ||
                                          permission.Name == Permissions.ClubsEdit ||
                                          permission.Name == Permissions.ClubsSave ||
                                          permission.Name == Permissions.Heroes ||
                                          permission.Name == Permissions.HeroesAdd ||
                                          permission.Name == Permissions.HeroesDelete ||
                                          permission.Name == Permissions.HeroesEdit ||
                                          permission.Name == Permissions.HeroesSave ||
                                          permission.Name == Permissions.HeroesAddCoordinator ||
                                          permission.Name == Permissions.Missions ||
                                          permission.Name == Permissions.MissionsAdd ||
                                          permission.Name == Permissions.MissionsDelete ||
                                          permission.Name == Permissions.MissionsEdit ||
                                          permission.Name == Permissions.MissionsSave ||
                                          permission.Name == Permissions.Stories ||
                                          permission.Name == Permissions.StoriesAdd ||
                                          permission.Name == Permissions.StoriesDelete ||
                                          permission.Name == Permissions.StoriesEdit ||
                                          permission.Name == Permissions.StoriesSave ||
                                          permission.Name == Permission.Admin;

            if (isCoordinatorPermission) coordinatorPermissions.Add(permission);
        }

        return coordinatorPermissions;
    }

    private IEnumerable<PermissionItem> GetTimeheroesPermissions()
    {
        return App.Permissions.GetPermissions();
    }
}