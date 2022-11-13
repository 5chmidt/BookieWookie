﻿namespace BookieWookie.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Authentication;
    using BookieWookie.API.Helpers;
    using BookieWookie.API.Models;
    using BookieWookie.API.Services;

    /// <summary>
    /// User controller for login and CRUD operations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        private IConfiguration _configuration;

        /// <summary>
        /// Initialize a user controller. 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="configuration"></param>
        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        /// <summary>
        /// Verify username and password then return a JWT with user claims.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Update user profile information, (cannot update username).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete a user, only an authenticated user can delete their own profile.
        /// </summary>
        /// <param name="id">Id of the user to be removed.</param>
        /// <returns>Object of the user profile that was deleted.</returns>
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

        /// <summary>
        /// Gets a list of all active users.
        /// </summary>
        /// <returns>Array of user objects.</returns>
        [AuthorizeOwner]
        [HttpGet("get")]
        public IActionResult Get()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}
