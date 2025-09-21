using System;
using System.Collections.Generic;

namespace LibraryManagement.Infrastructure.Persistence.Scaffolded.Entities;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string? Isbn { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int CategoryId { get; set; }

    public DateTime? PublishedDate { get; set; }

    public int Quantity { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
