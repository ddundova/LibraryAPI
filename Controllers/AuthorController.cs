using LibraryAPI.DTOs;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly LibraryContext _context;

        public AuthorController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _context.Authors
                .Select(a => new AuthorGetDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    BirthDate = a.BirthDate,
                    Books = a.Books.Select(b => new BookDto
                    {
                        Id = b.Id,
                        Title = b.Title
                    }).ToList()
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var author = _context.Authors
                .Select(a => new AuthorGetDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    BirthDate = a.BirthDate,
                    Books = a.Books.Select(b => new BookDto
                    {
                        Id = b.Id,
                        Title = b.Title
                    }).ToList()
                })
                .FirstOrDefault(a => a.Id == id);

            if (author == null)
            {
                return NotFound(new
                {
                    status = 400,
                    message = $"Author with ID {id} not found"
                });
            }

            return Ok(author);
        }

        [HttpPost]
        public IActionResult AddAuthor([FromBody] AuthorCreateDto authorDto)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Name == authorDto.Name);
            
            if (author != null)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = $"Author {authorDto.Name} already exists"
                });
            }

            author = new Author
            {
                Name = authorDto.Name,
                BirthDate = authorDto.BirthDate
            };

            _context.Authors.Add(author);
            _context.SaveChanges();

            return Ok(new AuthorCreateDto
            {
                Name = author.Name,
                BirthDate = author.BirthDate
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAuthor([FromRoute] int id, [FromBody] AuthorCreateDto authorDto)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == id);

            if(author == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = $"Author with ID {id} not found"
                });
            }

            author.Name = authorDto.Name;
            author.BirthDate = authorDto.BirthDate;
            _context.SaveChanges();

            return Ok(new AuthorCreateDto
            {
                Name = author.Name,
                BirthDate = author.BirthDate
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor([FromRoute] int id)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == id);

            if(author == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = $"Author with ID {id} not found"
                });
            }

            _context.Authors.Remove(author);
            _context.SaveChanges();

            return Ok($"Author with ID = {id} has been deleted successfully!");
        }
    }
}
