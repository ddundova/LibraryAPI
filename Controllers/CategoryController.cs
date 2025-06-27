using LibraryAPI.DTOs;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        LibraryContext _context = new LibraryContext();

        public CategoryController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult getAll()
        {
            var result = _context.Categories.ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult getById([FromRoute]int id)
        {
            var category = _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Books = c.Books.Select(b => new
                    {
                        b.Id,
                        b.Title
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
        public IActionResult addCategory([FromBody] string name)
        {
            Category? category = _context.Categories.FirstOrDefault(c => c.Name == name);

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
        public IActionResult updateCategory([FromBody] CategoryUpdateDto categoryDto)
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
        public IActionResult deleteCategory([FromRoute] int id) 
        { 
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound($"Category with id = {id} doesn't exist!");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return Ok($"Category with id = {id} has been successfully deleted!");
        }
    }
}
