namespace LibraryAPI.DTOs
{
    public class BookUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PublishedYear { get; set; }
        public int AuthorId { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
