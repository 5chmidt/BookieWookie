namespace BookieWookie.API.Helpers
{
    using BookieWookie.API.Services;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split("Bearer ").Last();
            if (token != null)
            {
                attachUserToContext(context, userService, token);
            }

            await _next(context);
        }

        private void attachUserToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(
                    ConfigurationManager.AppSetting["JWT:Secret"]);
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principle = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == nameof(Entities.User.UserId)).Value);
                
                // parse permission level from token //
                var permission = API.Authorization.PermissionLevel.None;
                string permissionName = jwtToken.Claims.First(x => x.Type == nameof(API.Authorization.PermissionLevel)).Value;
                Enum.TryParse(permissionName, out permission);

                // attach to context on successful jwt validation
                context.Items[nameof(Entities.User)] = userService.GetById(userId);
                context.Items[nameof(API.Authorization.PermissionLevel)] = permission;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }


    }
}