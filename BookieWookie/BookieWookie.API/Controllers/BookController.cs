using BookieWookie.API.Helpers;
using BookieWookie.API.Models;
using BookieWookie.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;


namespace BookieWookie.API.Controllers
{
    [Route("[controller]")]
    public class BookController : Controller
    {
        private IBookService _bookService;
        private IConfiguration _configuration;

        public BookController(IBookService bookService, IConfiguration configuration)
        {
            _bookService = bookService;
            _configuration = configuration;
        }

        [HttpGet("get")]
        public IActionResult Get()
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public IActionResult Create(CreateBookRequest model)
        {
            Entities.Book book;
            try
            {
                book = _bookService.Create(model);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(book);
        }

        [HttpPost("update")]
        public IActionResult Update()
        {
            throw new NotImplementedException();
        }

        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
