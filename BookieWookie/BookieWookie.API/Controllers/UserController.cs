namespace BookieWookie.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Authentication;
    using BookieWookie.API.Helpers;
    using BookieWookie.API.Models;
    using BookieWookie.API.Services;

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        private IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            if (response == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(response);
        }

        [HttpPost("create")]
        public IActionResult Create(UserRequest model)
        {
            JWTTokenResponse responce;
            try
            {
                responce = _userService.CreateUser(model);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }


            return Ok(responce);
        }

        [AuthorizeOwner]
        [HttpPost("update")]
        public IActionResult Update(UserRequest model)
        {
            Entities.User user;
            try
            {
                user = _userService.UpdateUser(model);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(model);
        }

        [AuthorizeOwner]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Entities.User model;
            try
            {
                model = _userService.DeleteUser(id);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(model);
        }

        [AuthorizeOwner]
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}
