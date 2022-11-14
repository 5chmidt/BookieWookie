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
    using System.Reflection;
    using System.Security;
    using System.Security.Authentication;
    using System.Security.Claims;
    using System.Text;

    public interface IUserService
    {
        JWTTokenResponse Authenticate(AuthenticateRequest model);
        JWTTokenResponse CreateUser(UserRequest model);
        User DeleteUser(int id);
        User UpdateUser(UserRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        private readonly IConfiguration Configuration;

        private readonly IAuthenticationService Authentication;

        public UserService(IAuthenticationService authenticationService, IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Authentication = authenticationService;
        }

        public JWTTokenResponse Authenticate(AuthenticateRequest model)
        {
            // find user in database //
            User user;
            using (var db = new BookieWookieContext(this.Configuration))
            {
                user = db.Users.SingleOrDefault(x => x.Username == model.Username);
            }

            // return null if user not found
            if (user == null)
            {
                return null;
            }

            // verify the hashed password //
            if (this.Authentication.VerifyHash(model.Password, user.Salt, user.Hash) == false)
            {
                return null;
            }

            var permission = Authorization.Authorize.GetPermissionLevel(user);
            Claim[] claims = new Claim[]
            {
                new Claim(nameof(User.UserId), user.UserId.ToString(), ClaimValueTypes.String),
                new Claim(nameof(User.Username), user.Username, ClaimValueTypes.String),
                new Claim(nameof(Authorization.PermissionLevel), permission.ToString(), ClaimValueTypes.String),
            };

            return IssueToken(claims);
        }

        public JWTTokenResponse CreateUser(UserRequest model)
        {
            var user = new User();
            using (var db = new BookieWookieContext(this.Configuration))
            {
                if (db.Users.Where(u => u.Username == model.Username).Count() > 0)
                {
                    throw new AuthenticationException($"Username '{model.Username}' already exists");
                }

                // throw exception if checks do not pass //
                AuthenticationService.CheckPasswordRequirements(model.Password);

                user.Username = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Pseudonym = model.Pseudonym;
                user.Salt = this.Authentication.CreateSalt();
                user.Hash = this.Authentication.HashPassword(model.Password, user.Salt);
                db.Users.Add(user);
                db.SaveChanges();
            }

            var permission = Authorization.Authorize.GetPermissionLevel(user);
            Claim[] claims = new Claim[]
            {
                new Claim(nameof(User.UserId), user.UserId.ToString(), ClaimValueTypes.String),
                new Claim(nameof(User.Username), user.Username, ClaimValueTypes.String),
                new Claim(nameof(Authorization.PermissionLevel), permission.ToString(), ClaimValueTypes.String),
            };
            return IssueToken(claims);
        }

        public User UpdateUser(UserRequest model)
        {
            var user = new User();
            using (var db = new BookieWookieContext(this.Configuration))
            {
                user = db.Users.SingleOrDefault(u => u.UserId == model.UserId);
                if (user == null)
                {
                    throw new AuthenticationException($"Username '{model.Username}' does not exist.");
                }

                bool updated = false;
                foreach (PropertyInfo modelProperty in model.GetType().GetRuntimeProperties())
                {
                    // cannot update userID //
                    if (modelProperty.Name == nameof(model.UserId))
                    {
                        continue;
                    }

                    object? value = modelProperty.GetValue(model, null);
                    if (value == null)
                    {
                        continue;
                    }

                    var userProperty = user.GetType().GetProperty(modelProperty.Name);
                    if (userProperty == null || userProperty.PropertyType != modelProperty.PropertyType)
                    {
                        continue;
                    }

                    if (modelProperty.Name == nameof(model.Password))
                    {
                        // special handling for password updates //
                        user.Salt = this.Authentication.CreateSalt();
                        user.Hash = this.Authentication.HashPassword(model.Password, user.Salt);
                    }
                    else if (modelProperty.Name == nameof(model.Username))
                    {
                        // check that new username is not already in use //
                        if (db.Users.Where(u => u.Username == model.Username).Count() > 0)
                        {
                            throw new AuthenticationException($"Username '{model.Username}' already exists");
                        }

                        userProperty.SetValue(model.Username, user);
                    }
                    else
                    {
                        // normal property update //
                        userProperty.SetValue(modelProperty.GetValue(model), user);
                    }

                    updated = true;
                }

                db.SaveChanges();
            }

            return user;
        }

        public User DeleteUser(int id)
        {
            var user = new User();
            using (var db = new BookieWookieContext(this.Configuration))
            {
                user = db.Users.SingleOrDefault(u => u.UserId == id);
                if (user == null)
                {
                    throw new AuthenticationException($"User ID: {id} does not exist.");
                }

                db.Users.Remove(user);
                db.SaveChanges();
            }

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            using (var db = new BookieWookieContext(this.Configuration))
            {
                return db.Users.ToArray();
            }
        }

        public User GetById(int id)
        {
            using (var db = new BookieWookieContext(this.Configuration))
            {
                return db.Users.Single(u => u.UserId == id);
            }
        }

        private JWTTokenResponse IssueToken(Claim[] claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                API.ConfigurationManager.AppSetting["JWT:Secret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: API.ConfigurationManager.AppSetting["JWT:ValidIssuer"],
                audience: API.ConfigurationManager.AppSetting["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signinCredentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new JWTTokenResponse { Token = tokenString };
        }
    }
}
