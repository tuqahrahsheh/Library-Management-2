using LibraryManagement.Application.Abstractions.Repositories;
using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Persistence
{
    public class LibraryDbContext : DbContext, IUnitOfWork
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<BookCategory> BookCategories => Set<BookCategory>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<BookCategory>().HasKey(x => new { x.BookId, x.CategoryId });

            b.Entity<BookCategory>()
                .HasOne(x => x.Book)
                .WithMany(x => x.BookCategories)
                .HasForeignKey(x => x.BookId);

            b.Entity<BookCategory>()
                .HasOne(x => x.Category)
                .WithMany(x => x.BookCategories)
                .HasForeignKey(x => x.CategoryId);

            b.Entity<Category>().HasIndex(x => x.Name).IsUnique();

            base.OnModelCreating(b);
        }
    }
}
