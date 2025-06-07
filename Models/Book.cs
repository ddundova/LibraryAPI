using System;
using System.Collections.Generic;

namespace LibraryAPI.Models;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int PublishedYear { get; set; }

    public int AuthorId { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
