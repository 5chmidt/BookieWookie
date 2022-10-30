namespace BookieWookie.API.Services
{
    using BookieWookie.API.Entities;
    using BookieWookie.API.Helpers;
    using BookieWookie.API.Models;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Authentication;
    using System.Security.Claims;
    using System.Text;

    public interface IUserService
    {
        JWTTokenResponse Authenticate(AuthenticateRequest model);
        JWTTokenResponse CreateUser(CreateUserRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration Configuration;


        public UserService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        [HttpPost("authenticate")]
        public JWTTokenResponse Authenticate(AuthenticateRequest model)
        {
            // find user in database //
            User user;
            using (var db = new WookieBookieContext(this.Configuration))
            {
                user = db.Users.SingleOrDefault(x => x.Username == model.Username);
            }

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            // verify the hashed password //
            var authService = new AuthenticationService();
            if (authService.VerifyHash(model.Password, user.Salt, user.Hash) == false)
            {
                return null;
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                API.ConfigurationManager.AppSetting["JWT:Secret"]));

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: API.ConfigurationManager.AppSetting["JWT:ValidIssuer"],
                audience: API.ConfigurationManager.AppSetting["JWT:ValidAudience"],
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return new JWTTokenResponse { Token = tokenString };
        }

        public JWTTokenResponse CreateUser(CreateUserRequest model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
