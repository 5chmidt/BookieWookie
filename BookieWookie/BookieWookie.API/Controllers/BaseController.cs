using BookieWookie.API.Authorization;
using BookieWookie.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookieWookie.API.Controllers
{
    /// <summary>
    /// Base class to parse user id and permission from context.
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Helper method to parse the current context's user id.
        /// </summary>
        /// <returns>Int - User ID.</returns>
        public int ParseUserIdFromContext
        {
            get
            {
                var user = (User?)this.HttpContext.Items[nameof(User)];
                return user == null ? 0 : Convert.ToInt32(user.UserId);
            }
        }

        /// <summary>
        /// Helper method to parse the current user's permissions.
        /// </summary>
        /// <returns><see cref="PermissionLevel"/></returns>
        public PermissionLevel ParsePermissionFromContext
        {
            get
            {
                var permission = (PermissionLevel?)this.HttpContext.Items[nameof(PermissionLevel)];
                return permission == null ? PermissionLevel.None : (PermissionLevel)permission;
            }
        }
    }
}
