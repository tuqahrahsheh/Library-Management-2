using LibraryManagement.Application.Abstractions.Repositories;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class BookRepositoryEF : IBookRepository
    {
        private readonly LibraryDbContext _db;
        public BookRepositoryEF(LibraryDbContext db) => _db = db;

        public Task<Book?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _db.Books.Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                     .FirstOrDefaultAsync(b => b.BookId == id, ct);

        public async Task AddAsync(Book book, CancellationToken ct = default) =>
            await _db.Books.AddAsync(book, ct);

        public Task RemoveAsync(Book book, CancellationToken ct = default)
        { _db.Books.Remove(book); return Task.CompletedTask; }

        public Task<List<Book>> ListAsync(CancellationToken ct = default) =>
            _db.Books.Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
                     .OrderBy(b => b.Title)
                     .ToListAsync(ct);
    }
}
