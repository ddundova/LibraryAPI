using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs
{
    public class BookCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PublishedYear must be valid")]
        public int PublishedYear { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AuthorId must be greater than 0")]
        public int AuthorId { get; set; }

        [MinLength(1, ErrorMessage = "At least one category is required")]
        public List<int> CategoryIds { get; set; }
    }
}
