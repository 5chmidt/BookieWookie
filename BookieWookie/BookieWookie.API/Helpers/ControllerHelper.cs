namespace BookieWookie.API.Helpers
{
    using BookieWookie.API.Entities;
    using Microsoft.AspNetCore.Mvc;

    internal static class ControllerHelper
    {
        /// <summary>
        /// Helper method to parse the current context's user id.
        /// </summary>
        /// <param name="controller">The controller being used currently.</param>
        /// <returns>Int - User ID.</returns>
        public static int ParseUserIdFromContext(this Controller controller)
        {
            var user = (User?)controller.HttpContext.Items[nameof(User)];
            return user == null ? 0 : Convert.ToInt32(user.UserId);
        }
    }
}
