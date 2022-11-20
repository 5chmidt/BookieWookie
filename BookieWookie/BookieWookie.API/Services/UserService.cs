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

    /// <summary>
    /// User service implimenting Authentication and CRUD operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticate hashed credentials and return jwt token if successful.
        /// </summary>
        /// <param name="model">Authentication request contains a username/password</param>
        /// <returns>JWT token if user was successfully authenticated.</returns>
        Task<JWTTokenResponse?> Authenticate(AuthenticateRequest model);

        /// <summary>
        /// Create a new user and securely stores credentials.
        /// </summary>
        /// <param name="model"><see cref="UpdateUserRequest"/></param>
        /// <returns>JWT token if user was successfully created.</returns>
        JWTTokenResponse CreateUser(CreateUserRequest model);

        /// <summary>
        /// Deletes a user from the database, (users can only remove themselves).
        /// </summary>
        /// <param name="id">Unique identifier for user.</param>
        /// <param name="userId">Currently logged in user.</param>
        /// <returns>User object that was deleted.</returns>
        User DeleteUser(int id, int userId);

        /// <summary>
        /// Updates a user model, if new password input rehash/salt credentials.
        /// </summary>
        /// <param name="model"><see cref="UpdateUserRequest"/></param>
        /// <param name="userId">Id of currently authenticated user.</param>
        /// <returns><see cref="User"/></returns>
        User UpdateUser(UpdateUserRequest model, int userId);
        
        /// <summary>
        /// Admin only function hidden from swagger UI to get all users.
        /// </summary>
        /// <returns>Collection of user objects.</returns>
        IEnumerable<User> GetAll();
        
        /// <summary>
        /// Gets information of a single user by user id.
        /// </summary>
        /// <param name="id">Unique identifier for each user.</param>
        /// <returns><see cref="User"/></returns>
        User GetById(int id);
    }

    /// <inheritdoc/>
    public class UserService : IUserService
    {
        private readonly IConfiguration Configuration;

        private readonly IAuthenticationService Authentication;

        /// <summary>
        /// Initialize user service with dependency injection.
        /// </summary>
        /// <param name="authenticationService">Authentication service used to for secure password hashing.</param>
        /// <param name="configuration">Configuration used when making db queries.</param>
        public UserService(IAuthenticationService authenticationService, IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Authentication = authenticationService;
        }
        
        /// <inheritdoc/>
        public async Task<JWTTokenResponse?> Authenticate(AuthenticateRequest model)
        {
            // find user in database //
            User? user;
            using (var db = new BookieWookieContext(this.Configuration))
            {
                user = await db.Users.SingleOrDefaultAsync(x => x.Username.ToLower() == model.Username.ToLower());
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

        /// <inheritdoc/>
        public JWTTokenResponse CreateUser(CreateUserRequest model)
        {
            //username is not case sensitive //
            model.Username = model.Username.ToLower();

            if (string.IsNullOrEmpty(model.Username))
            {
                throw new AuthenticationException($"Username cannot be null.");
            }

            string regex = @"/^[0-Z]{1,50}$/";
            if (System.Text.RegularExpressions.Regex.IsMatch(model.Username, regex) == false)
            {
                throw new AuthenticationException($"Username can only contian letters and numbers");
            }

            var user = new User();
            using (var db = new BookieWookieContext(this.Configuration))
            {
                // throw exception if checks do not pass //
                if (db.Users.Where(u => u.Username == model.Username).Count() > 0)
                {
                    throw new AuthenticationException($"Username '{model.Username}' already exists");
                }
             
                user.Username = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Pseudonym = model.Pseudonym;
                
                if (string.IsNullOrEmpty(model.Password) == false)
                {
                    AuthenticationService.CheckPasswordRequirements(model.Password);
                    user.Salt = this.Authentication.CreateSalt();
                    user.Hash = this.Authentication.HashPassword(model.Password, user.Salt);
                }

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
        
        /// <inheritdoc/>
        public User UpdateUser(UpdateUserRequest model, int userId)
        {
            //username is not case sensitive //
            model.Username = model.Username.ToLower();

            if (model.UserId != userId)
            {
                throw new AuthenticationException("Users can only update their own profile.");
            }

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

                if (updated)
                {
                    db.SaveChanges();
                }
            }

            return user;
        }
        
        /// <inheritdoc/>
        public User DeleteUser(int id, int userId)
        {
            if (id != userId)
            {
                throw new AuthenticationException($"Users can only delete themselves.");
            }

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
        
        /// <inheritdoc/>
        public IEnumerable<User> GetAll()
        {
            using (var db = new BookieWookieContext(this.Configuration))
            {
                return db.Users.ToArray();
            }
        }
        
        /// <inheritdoc/>
        public User GetById(int id)
        {
            using (var db = new BookieWookieContext(this.Configuration))
            {
                return db.Users.Single(u => u.UserId == id);
            }
        }
        
        /// <inheritdoc/>
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
