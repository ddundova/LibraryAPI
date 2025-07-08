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
                .Select(b => new BookGetDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    PublishedYear = b.PublishedYear,
                    AuthorId = b.Author.Id,
                    AuthorName = b.Author.Name,
                    Categories = b.Categories.Select(c => c.Name).ToList()
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
                .Select(b => new BookGetDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Description = b.Description,
                    PublishedYear = b.PublishedYear,
                    AuthorId = b.Author.Id,
                    AuthorName = b.Author.Name,
                    Categories = b.Categories.Select(c => c.Name).ToList()
                }).FirstOrDefault();

            if (book == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = $"Book with ID {id} not found"
                });
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] BookCreateDto bookDto)
        {
            var existing = _context.Books
                .FirstOrDefault(b =>
                b.Title.ToLower() == bookDto.Title.ToLower() &&
                b.AuthorId == bookDto.AuthorId &&
                b.PublishedYear == bookDto.PublishedYear);

            if (existing != null)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = $"Book {bookDto.Title} already exists"
                });
            }

            var author = _context.Authors.Find(bookDto.AuthorId);
            if (author == null)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = $"Author with ID {bookDto.AuthorId} does not exist"
                });
            }

            var categories = _context.Categories
                .Where(c => bookDto.CategoryIds.Contains(c.Id))
                .ToList();
            if (categories.Count != bookDto.CategoryIds.Count)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "One or more categories entered wrong"
                });
            }

            var newBook = new Book
            {
                Title = bookDto.Title,
                Description = bookDto.Description,
                PublishedYear = bookDto.PublishedYear,
                AuthorId = bookDto.AuthorId,
                Categories = categories
            };

            _context.Books.Add(newBook);
            _context.SaveChanges();

            return Ok(new BookAddOutputDto
            {
                Id = newBook.Id,
                Title = newBook.Title,
                AuthorName = newBook.Author.Name,
                Categories = newBook.Categories.Select(c => c.Name).ToList()
            });

        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook([FromRoute] int id, [FromBody] BookCreateDto bookDto)
        {
            var book = _context.Books
                .Include(b => b.Categories)
                .FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = $"Book with ID {id} not found"
                });
            }

            var author = _context.Authors.FirstOrDefault(a => a.Id == bookDto.AuthorId);
            if (author == null)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = $"Author with ID {bookDto.AuthorId} does not exist"
                });
            }

            var categories = _context.Categories
                .Where(c => bookDto.CategoryIds.Contains(c.Id))
                .ToList();
            if (categories.Count != bookDto.CategoryIds.Count)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "One or more categories entered wrong"
                });
            }

            book.Title = bookDto.Title;
            book.Description = bookDto.Description;
            book.PublishedYear = bookDto.PublishedYear;
            book.AuthorId = bookDto.AuthorId;

            List<int> bookCategoryIds = book.Categories.Select(c => c.Id).OrderBy(id => id).ToList();
            List<int> bookDtoCategoryIds = bookDto.CategoryIds.OrderBy(id => id).ToList();
            bool isCategoriesChanged = false;

            if (bookCategoryIds.Count != bookDtoCategoryIds.Count)
            {
                isCategoriesChanged = true;
            }
            else
            {
                for (int i = 0; i < bookCategoryIds.Count; i++)
                {
                    if (bookCategoryIds[i] != bookDtoCategoryIds[i])
                    {
                        isCategoriesChanged = true;
                        break;
                    }
                }
            }

            if (isCategoriesChanged)
            {
                book.Categories.Clear();
                foreach (var category in categories)
                {
                    book.Categories.Add(category);
                }
            }
            _context.SaveChanges();

            return Ok(new BookAddOutputDto
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Author.Name,
                Categories = book.Categories.Select(c => c.Name).ToList()
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook([FromRoute] int id)
        {
            var book = _context.Books
                .Include(b => b.Categories)
                .FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = $"Book with ID {id} not found"
                });
            }

            book.Categories.Clear();

            _context.Remove(book);
            _context.SaveChanges();
            return Ok($"Book with ID = {id} was deleted successfully");
        }

        [HttpGet("search")]
        public IActionResult SearchByTitle([FromQuery] string title)
        {
            if (string.IsNullOrEmpty(title)) {
                return BadRequest(new
                {
                    status = 400,
                    message = "Title should not be empty"
                });
            }

            var books = _context.Books
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title
                })
                .Where(b => b.Title.ToLower().Contains(title.ToLower()))
                .ToList();

            return Ok(books);

        }
    }
}
