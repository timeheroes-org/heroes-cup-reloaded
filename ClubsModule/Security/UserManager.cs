using System.Security.Claims;

namespace HeroesCup.Web.ClubsModule.Security;

public class UserManager : IUserManager
{
    public const string SYS_ADMIN_ROLE = "SysAdmin";
    public const string TIMEHEROES_ROLE = "Timeheroes";
    private readonly HttpContext context;
    private readonly ClaimsPrincipal user;

    public UserManager(IHttpContextAccessor httpAccess)
    {
        context = httpAccess.HttpContext;
        user = context.User;
    }

    public bool IsCurrentUserAdmin()
    {
        return user.IsInRole(SYS_ADMIN_ROLE) || user.IsInRole(TIMEHEROES_ROLE);
    }

    public Guid? GetCurrentUserId()
    {
        if (!user.Identity.IsAuthenticated) return Guid.Empty;

        if (IsCurrentUserAdmin()) return null;

        return new Guid(user.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}