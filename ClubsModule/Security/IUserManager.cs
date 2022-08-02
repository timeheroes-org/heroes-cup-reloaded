using System;

namespace HeroesCup.Web.ClubsModule.Security
{
    public interface IUserManager
    {
        bool IsCurrentUserAdmin();

        Guid? GetCurrentUserId();
    }
}