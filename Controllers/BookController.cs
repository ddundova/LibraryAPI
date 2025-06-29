using LibraryAPI.DTOs;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BookController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .Select(b => new 
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.PublishedYear,
                    b.AuthorId,
                    Author = new
                    {
                        b.Author.Name
                    },
                    Categories = b.Categories.Select(c => new
                    {
                        c.Name
                    }).ToList()
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var book = _context.Books
                .Where(b => b.Id == id)
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.PublishedYear,
                    b.AuthorId,
                    Author = new
                    {
                        b.Author.Name
                    },
                    Categories = b.Categories.Select(c => new
                    {
                        c.Name
                    }).ToList()
                }).FirstOrDefault();

            if (book == null)
            {
                return NotFound($"No book with id = {id} was found!");
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] BookCreateDto bookDto)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == bookDto.AuthorId);
            if (author == null)
            {
                return BadRequest($"Author with id = {bookDto.AuthorId} does not exist.");
            }

            var categories = _context.Categories
                .Where(c => bookDto.CategoryIds.Contains(c.Id))
                .ToList();
            if (categories.Count != bookDto.CategoryIds.Count)
            {
                return BadRequest("One or more category IDs are invalid.");
            }

            var newBook = new Book
            {
                Title = bookDto.Title,
                Description = bookDto.Description,
                PublishedYear = bookDto.PublishedYear,
                AuthorId = author.Id,
                Categories = categories
            };

            _context.Books.Add(newBook);
            _context.SaveChanges();
            return Ok(newBook);
        }

        [HttpPut]
        public IActionResult UpdateBook([FromBody] BookUpdateDto bookDto)
        {
            var book = _context.Books
                .Include(b => b.Categories)
                .FirstOrDefault(b => b.Id == bookDto.Id);
            if (book == null)
            {
                return NotFound($"Book with id = {bookDto.Id} not found.");
            }

            var author = _context.Authors.FirstOrDefault(a => a.Id == bookDto.AuthorId);
            if (author == null)
            {
                return BadRequest($"Author with id = {bookDto.AuthorId} does not exist.");
            }

            var categories = _context.Categories
                .Where(c => bookDto.CategoryIds.Contains(c.Id))
                .ToList();
            if (categories.Count != bookDto.CategoryIds.Count)
            {
                return BadRequest("One or more category IDs are invalid.");
            }

            book.Title = bookDto.Title;
            book.Description = bookDto.Description;
            book.PublishedYear = bookDto.PublishedYear;
            book.AuthorId = bookDto.AuthorId;
            book.Categories.Clear();
            foreach (var category in categories)
            {
                book.Categories.Add(category);
            }

            _context.SaveChanges();
            return Ok($"Book with id = {bookDto.Id} was updated successfully.");
        }

        [HttpDelete]
        public IActionResult DeleteBook([FromRoute] int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound($"Book with id = {id} was not found.");
            }

            _context.Remove(book);
            _context.SaveChanges();
            return Ok($"Book with id = {id} was deleted successfully.");
        }
    }
}
