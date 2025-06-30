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
                return NotFound($"No category found with id = {id}!");
            }
            return Ok(category);
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] string name)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name == name);

            if (category != null)
            {
                return BadRequest($"The category {name} already exists!");
            }

            Category newCategory = new Category { Name = name };
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return Ok("Category has been added!");
        }

        [HttpPut]
        public IActionResult UpdateCategory([FromBody] CategoryUpdateDto categoryDto)
        {
            var updatedCategory = _context.Categories.FirstOrDefault(c => c.Id == categoryDto.Id);
            if (updatedCategory == null)
            {
                return NotFound($"Category with id = {categoryDto.Id} doesn't exist!");
            }

            updatedCategory.Name = categoryDto.Name;
            _context.SaveChanges();
            return Ok($"Category with id = {categoryDto.Id} has been successfully updated!");            
        }

        [HttpDelete]
        public IActionResult DeleteCategory([FromRoute] int id) 
        { 
            var category = _context.Categories
                .Include(c => c.Books)
                .FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound($"Category with id = {id} doesn't exist!");
            }

            category.Books.Clear();

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Ok($"Category with id = {id} has been successfully deleted!");
        }
    }
}
