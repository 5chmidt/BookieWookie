namespace BookieWookie.API.Helpers
{
    using BookieWookie.API.Authorization;
    using BookieWookie.API.Entities;
    using Microsoft.AspNetCore.Mvc;

    internal static class ControllerHelper
    {
        /// <summary>
        /// Helper method to parse the current context's user id.
        /// </summary>
        /// <param name="controller">The controller being used currently.</param>
        /// <returns>Int - User ID.</returns>
        public static int ParseUserIdFromContext(this ControllerBase controller)
        {
            var user = (User?)controller.HttpContext.Items[nameof(User)];
            return user == null ? 0 : Convert.ToInt32(user.UserId);
        }

        /// <summary>
        /// Helper method to parse the current user's permissions.
        /// </summary>
        /// <param name="controller"><see cref="ControllerBase"/></param>
        /// <returns><see cref="PermissionLevel"/></returns>
        public static PermissionLevel ParsePermissionFromContext(this ControllerBase controller)
        {
            var permission = (PermissionLevel?)controller.HttpContext.Items[nameof(PermissionLevel)];
            return permission == null ? PermissionLevel.None : (PermissionLevel)permission;
        }
    }
}
