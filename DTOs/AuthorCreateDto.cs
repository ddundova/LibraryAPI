using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs
{
    public class AuthorCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "BirthDate is required")]
        [DataType(DataType.Date, ErrorMessage = "BirthDate must be a valid date")]
        public DateTime BirthDate { get; set; }

    }
}
