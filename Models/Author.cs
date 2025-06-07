using System;
using System.Collections.Generic;

namespace LibraryAPI.Models;

public partial class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
