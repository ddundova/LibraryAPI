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
                return NotFound($"No author found with id = {id}!");
            }
            return Ok(author);
        }

        [HttpPost]
        public IActionResult AddAuthor([FromBody] AuthorCreateDto authorDto)
        {
            var author = new Author
            {
                Name = authorDto.Name,
                BirthDate = authorDto.BirthDate
            };

            _context.Authors.Add(author);
            _context.SaveChanges();
            return Ok($"Author: {authorDto.Name} has been added successfully!");
        }

        [HttpPut]
        public IActionResult UpdateAuthor([FromBody] AuthorUpdateDto authorDto)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == authorDto.Id);

            if(author == null)
            {
                return NotFound($"No author with id = {authorDto.Id} was found!");
            }

            author.Name = authorDto.Name;
            author.BirthDate = authorDto.BirthDate;

            _context.SaveChanges();
            return Ok($"Author with id = {author.Id} has been updated successfully!");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor([FromBody] int id)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == id);

            if(author == null)
            {
                return NotFound($"No author with id = {id} was found!");
            }

            _context.Authors.Remove(author);
            _context.SaveChanges();

            return Ok($"Author with id = {id} has been deleted successfully!");
        }
    }
}
