namespace LibraryAPI.DTOs
{
    public class CategoryGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BookDto> Books {  get; set; } 
    }
}
