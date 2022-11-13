using BookieWookie.API.Authorization;
using BookieWookie.API.Entities;
using BookieWookie.API.Helpers;
using BookieWookie.API.Models;
using BookieWookie.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;


namespace BookieWookie.API.Controllers
{
    /// <summary>
    /// Controller for CRUD operations on the book database.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BookController : Controller
    {
        private IUserService _userService;
        private IBookService _bookService;
        private IConfiguration _configuration;

        /// <summary>
        /// Book controller for API CRUD operations.
        /// </summary>
        /// <param name="bookService">Book service with business logig.</param>
        /// <param name="configuration">Configuration settings for API web app.</param>
        /// <param name="userService">Service to authorize/ authenticate users.</param>
        public BookController(IBookService bookService, IConfiguration configuration, IUserService userService)
        {
            _bookService = bookService;
            _configuration = configuration;
            _userService = userService;
        }

        /// <summary>
        /// Returns a list of books from query parameters.
        /// </summary>
        /// <param name="bookParams">Optional parameters for book queries.</param>
        /// <returns></returns>
        [AuthorizeOwner]
        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] BookParameters bookParams)
        {
            IEnumerable<Book> books;
            try
            {
                books = await _bookService.Get(bookParams);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(books);
        }

        /// <summary>
        /// Allows a user to add a book to the database.
        /// </summary>
        /// <param name="model">Book creation request, the authorized user will be added as the owner.</param>
        /// <returns></returns>
        [AuthorizeOwner]
        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateBookRequest model)
        {
            Book book;
            try
            {
                book = await _bookService.Create(model, UserId);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(book);
        }

        /// <summary>
        /// Update an existing book, users an only update books they published.
        /// </summary>
        /// <param name="book">Book object to be updated.</param>
        /// <returns>The object that was updated.</returns>
        [AuthorizeOwner]
        [HttpPost("update")]
        public async Task<IActionResult> Update(Book book)
        {
            try
            {
                book = await _bookService.Update(book, UserId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(book);
        }

        /// <summary>
        /// Delete and existing book, users can only delete books they published.
        /// </summary>
        /// <param name="bookId">Id of the book to be deleted.</param>
        /// <returns>The object that was deleted.</returns>
        [AuthorizeOwner]
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> Delete(int bookId)
        {
            Book book;
            try
            {
                book = await _bookService.Delete(bookId, UserId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(book);
        }

        private int UserId { 
            get
            {
                var user = (User?)HttpContext.Items[nameof(Entities.User)];
                return user == null ? 0 : Convert.ToInt32(user.UserId);
            } 
        }

        private PermissionLevel UserPermission
        {
            get
            {
                var permission = (PermissionLevel?)HttpContext.Items[nameof(PermissionLevel)];
                return permission == null ? PermissionLevel.None : (PermissionLevel)permission;
            }
        }
    }
}
