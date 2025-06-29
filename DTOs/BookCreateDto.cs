using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs
{
    public class BookCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int PublishedYear { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public List<int> CategoryIds { get; set; }
    }
}
