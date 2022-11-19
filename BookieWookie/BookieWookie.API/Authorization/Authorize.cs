using BookieWookie.API.Entities;

namespace BookieWookie.API.Authorization
{
    /// <summary>
    /// Class for granting authoriation to users.
    /// </summary>
    public class Authorize
    {
        /// <summary>
        /// Static method to get simple permissions.
        /// To be replaced by permissions granted from database.
        /// </summary>
        /// <param name="user"><see cref="User"/></param>
        /// <returns><see cref="PermissionLevel"/></returns>
        public static PermissionLevel GetPermissionLevel(User user)
        {
            
            if (user == null)
            {
                // null users get no permissions //
                return PermissionLevel.None;
            }
            else if ((user.FirstName == "Anakin" && user.LastName == "Skywalker")
                || user.Pseudonym == "Darth Vader" 
                || user.Username == "_Darth Vader_")
            {
                // Darth Vader can only view books //
                return PermissionLevel.Get;
            }
            else if (user.Username == "Yoda")
            {
                // Designate system admin role //
                return PermissionLevel.Admin;
            }
            else
            {
                // Allow users delete their own records //
                return PermissionLevel.Delete;
            }
        }
    }
}
