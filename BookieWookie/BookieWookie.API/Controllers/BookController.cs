using BookieWookie.API.Authorization;
using BookieWookie.API.Entities;
using BookieWookie.API.Helpers;
using BookieWookie.API.Models;
using BookieWookie.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;


namespace BookieWookie.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : Controller
    {
        private IUserService _userService;
        private IBookService _bookService;
        private IConfiguration _configuration;

        public BookController(IBookService bookService, IConfiguration configuration, IUserService userService)
        {
            _bookService = bookService;
            _configuration = configuration;
            _userService = userService;
        }

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
