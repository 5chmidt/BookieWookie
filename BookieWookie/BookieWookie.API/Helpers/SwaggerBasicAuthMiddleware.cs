using System.Net.Http.Headers;
using System.Net;
using System.Text;
using BookieWookie.API.Entities;
using BookieWookie.API.Services;
using BookieWookie.API.Models;

namespace BookieWookie.API.Helpers
{
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate next;

        public SwaggerBasicAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
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
                        await next.Invoke(context).ConfigureAwait(false);
                        return;
                    }
                }
                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context).ConfigureAwait(false);
            }
        }
    }
}
