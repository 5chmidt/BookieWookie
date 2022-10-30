﻿namespace BookieWookie.API.Services
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
        JWTTokenResponse CreateUser(UserRequest model);
        UserRequest UpdateUser(UserRequest model);
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

            Claim[] claims = new Claim[0];
            return IssueToken(claims);
        }

        public JWTTokenResponse CreateUser(UserRequest model)
        {
            var user = new User();
            using (var db = new WookieBookieContext(this.Configuration))
            {
                if (db.Users.Where(u => u.Username == model.Username).Count() > 0)
                {
                    throw new AuthenticationException($"Username '{model.Username}' already exists");
                }

                user.Username = model.Username;
                var authService = new AuthenticationService();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Pseudonym = model.Pseudonym;
                user.Salt = authService.CreateSalt();
                user.Hash = authService.HashPassword(model.Password, user.Salt);
                db.Users.Add(user);
                db.SaveChanges();
            }

            Claim[] claims = new Claim[]
            {
                new Claim(nameof(User.Id), user.Id.ToString()),
                new Claim(nameof(User.Username), user.Username),
            };
            return IssueToken(claims);
        }

        public UserRequest UpdateUser(UserRequest model)
        {
            var user = new User();
            using (var db = new WookieBookieContext(this.Configuration))
            {
                user = db.Users.SingleOrDefault(u => u.Username == model.Username);
                if (user == null)
                {
                    throw new AuthenticationException($"Username '{model.Username}' does not exist.");
                }

                user.Username = model.Username;
                var authService = new AuthenticationService();
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Pseudonym = model.Pseudonym;
                user.Salt = authService.CreateSalt();
                user.Hash = authService.HashPassword(model.Password, user.Salt);
                db.SaveChanges();
            }

            Claim[] claims = new Claim[0];
            return IssueToken(claims);
        }

        public IEnumerable<User> GetAll()
        {
            using (var db = new WookieBookieContext(this.Configuration))
            {
                return db.Users.ToArray();
            }
        }

        public User GetById(int id)
        {
            using (var db = new WookieBookieContext(this.Configuration))
            {
                return db.Users.FirstOrDefault(u => u.Id == id);
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
