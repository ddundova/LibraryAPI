namespace LibraryAPI.DTOs
{
    public class BookAddOutputDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public List<string> Categories { get; set; }
    }
}
