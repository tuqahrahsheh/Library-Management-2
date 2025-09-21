using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Domain.Entities
{
    public class Category
    {
        public int  CategoryId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [NotMapped] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped] public DateTime? UpdatedAt { get; set; }

        // Many-to-many via join entity
        public virtual ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    }
}
