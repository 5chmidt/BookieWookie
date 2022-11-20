namespace BookieWookie.API.Controllers
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
        /// Gets a user's profile information by UserId.
        /// </summary>
        /// <param name="id">Interger identifier.</param>
        /// <returns>User profile model.</returns>
        [AuthorizeOwner]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Entities.User model;
            try
            {
                model = _userService.GetById(id);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(model);
        }

        /// <summary>
        /// Verify username and password then return a JWT with user claims.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model);
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
        [HttpPost("Create")]
        public IActionResult Create(CreateUserRequest model)
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
        /// Update user profile information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthorizeOwner]
        [HttpPost("Update")]
        public IActionResult Update(UpdateUserRequest model)
        {
            Entities.User user;
            try
            {
                int userId = this.ParseUserIdFromContext();
                user = _userService.UpdateUser(model, userId);
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
                int userId = this.ParseUserIdFromContext();
                model = _userService.DeleteUser(id, userId);
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
        [HttpGet("GetAll")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}
