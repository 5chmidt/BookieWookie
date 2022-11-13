using BookieWookie.API.Entities;

namespace BookieWookie.API.Authorization
{
    public class Authorize
    {
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
                return PermissionLevel.View;
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
