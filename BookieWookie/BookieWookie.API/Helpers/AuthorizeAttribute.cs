namespace BookieWookie.API.Helpers
{
    using BookieWookie.API.Authorization;
    using BookieWookie.API.Entities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeOwnerAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            User? user = context.HttpContext.Items[nameof(User)] as User;
            if (user == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            PermissionLevel authorizedPermission = PermissionLevel.None;
            if (context.HttpContext.Items.ContainsKey(nameof(PermissionLevel)) == false)
            {
                // no permission attached to HTTP context //
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }

            object? value = context.HttpContext.Items[nameof(PermissionLevel)];
            if (value != null && value.GetType() == typeof(PermissionLevel))
            {
                authorizedPermission = (PermissionLevel)value;
            }

            PermissionLevel requiredPermission = PermissionLevel.Admin;
            Enum.TryParse<PermissionLevel>(
                context.RouteData.Values["Action"].ToString(), 
                true,
                out requiredPermission);
            if (authorizedPermission < requiredPermission)
            {
                // user lacks the required permissions //
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                return;
            }
        }
    }
}
