﻿using BookieWookie.API.Helpers;
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
        public IActionResult Get()
        {
            IEnumerable<Entities.Book> books;
            try
            {
                books = _bookService.Get();
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(books);
        }

        [AuthorizeOwner]
        [HttpPost("create")]
        public IActionResult Create(CreateBookRequest model)
        {
            Entities.Book book;
            try
            {
                book = _bookService.Create(model, UserId);
            }
            catch (AuthenticationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(book);
        }

        [AuthorizeOwner]
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

        private int UserId { 
            get
            {
                var user = (Entities.User?)HttpContext.Items["User"];
                return user == null ? 0 : Convert.ToInt32(user.Id);
            } 
        }
    }
}
