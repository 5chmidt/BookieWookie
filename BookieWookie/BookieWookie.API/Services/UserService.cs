namespace BookieWookie.API.Services
{
    using BookieWookie.API.Entities;
    using BookieWookie.API.Helpers;
    using BookieWookie.API.Models;
    using Microsoft.AspNetCore.Components.Forms;
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

        public UserService()
        {

        }

        public JWTTokenResponse Authenticate(AuthenticateRequest model)
        {
            throw new NotImplementedException();
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
