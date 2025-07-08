using LibraryAPI.DTOs;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly LibraryContext _context;

        public CategoryController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _context.Categories
                .Select(c => new CategoryGetDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Books = c.Books.Select(b => new BookDto
                    {
                        Id = b.Id,
                        Title = b.Title
                    }).ToList()
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var category = _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryGetDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Books = c.Books.Select(b => new BookDto
                    {
                        Id = b.Id,
                        Title = b.Title
                    }).ToList()
                })
                .FirstOrDefault();

            if (category == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = $"Category with ID {id} not found"
                });
            }
            return Ok(category);
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] CategoryCreateDto categoryDto)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name == categoryDto.Name);

            if (category != null)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = $"Category {categoryDto.Name} already exists"
                });
            }

            Category newCategory = new Category { Name = categoryDto.Name };
            _context.Categories.Add(newCategory);
            _context.SaveChanges();

            return Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory([FromRoute] int id, [FromBody] CategoryCreateDto categoryDto)
        {
            var updatedCategory = _context.Categories.FirstOrDefault(c => c.Id == id);

            if (updatedCategory == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = $"Category with ID {id} not found"
                });
            }

            updatedCategory.Name = categoryDto.Name;
            _context.SaveChanges();

            return Ok(new CategoryDto
            {
                Id = updatedCategory.Id,
                Name = updatedCategory.Name
            });            
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory([FromRoute] int id) 
        { 
            var category = _context.Categories
                .Include(c => c.Books)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new
                {
                    status = 404,
                    message = $"Category with ID {id} not found"
                });
            }

            category.Books.Clear();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return Ok($"Category with ID = {id} has been successfully deleted!");
        }
    }
}
