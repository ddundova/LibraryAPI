namespace LibraryAPI.DTOs
{
    public class BookGetDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PublishedYear { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public List<string> Categories { get; set; }
    }
}
