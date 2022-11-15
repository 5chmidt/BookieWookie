using System.Net.Http.Headers;
using System.Net;
using System.Text;
using BookieWookie.API.Entities;
using BookieWookie.API.Services;
using BookieWookie.API.Models;

namespace BookieWookie.API.Helpers
{
    /// <summary>
    /// Middleware class to put up basic authentication before swagger page.
    /// </summary>
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// SwaggerBasicAuthMiddleware constructor.
        /// </summary>
        /// <param name="next">set the next request.</param>
        public SwaggerBasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// If authenticated proceed to swagger page, else login page.
        /// </summary>
        /// <param name="context">Current HttpContext.</param>
        /// <param name="userService">Dependency inject user managment service.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IUserService userService)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await this.displayDefaultLogin(context, userService);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task displayDefaultLogin(HttpContext context, IUserService userService)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the credentials from request header
                var header = AuthenticationHeaderValue.Parse(authHeader);
                var inBytes = Convert.FromBase64String(header.Parameter);
                var credentials = Encoding.UTF8.GetString(inBytes).Split(':');

                // validate credentials
                var authRequest = new AuthenticateRequest()
                {
                    Username = credentials[0],
                    Password = credentials[1],
                };

                var result = await userService.Authenticate(authRequest);
                if (result != null)
                {
                    await _next(context);
                    return;
                }
            }
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}
