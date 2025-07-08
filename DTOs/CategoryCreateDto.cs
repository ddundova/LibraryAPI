using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
