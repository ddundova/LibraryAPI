namespace LibraryAPI.DTOs
{
    public class AuthorGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public List<BookDto> Books { get; set; }
    }
}
