using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs
{
    public class AuthorUpdateDto
    {

        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

    }
}
