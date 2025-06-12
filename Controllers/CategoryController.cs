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
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound($"No category found with id = {id}!");
            }
            return Ok(category);
        }

        [HttpPost]
        public IActionResult addCategory([FromBody] string name)
        {
            Category? newCategory = _context.Categories.FirstOrDefault(c => c.Name == name);

            if (newCategory != null)
            {
                return BadRequest($"The category {name} already exists!");
            }

            _context.Categories.Add(newCategory);
            _context.SaveChanges();
            return Ok("Category has been added!");
        }

        [HttpPut]
        public IActionResult updateCategory([FromBody] int id, string newName)
        {
            Category? updatedCategory = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (updatedCategory != null)
            {
                updatedCategory.Name = newName;

                _context.SaveChanges();

                return Ok($"Category with id = {id} has been successfully updated!");
            }

            return NotFound($"Vehicle with id = {id} doesn't exist!");
        }

        [HttpDelete]
        public IActionResult deleteCategory([FromBody] int id) 
        { 
            Category? category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return Ok($"Category with id = {id} has been successfully deleted!");
            }
            return NotFound($"Category with id = {id} doesn't exist!");
        }
    }
}
