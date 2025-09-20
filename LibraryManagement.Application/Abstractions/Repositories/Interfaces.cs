using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Abstractions.Repositories
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Book book, CancellationToken ct = default);
        Task RemoveAsync(Book book, CancellationToken ct = default);
        Task<List<Book>> ListAsync(CancellationToken ct = default);
    }

    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Category category, CancellationToken ct = default);
        Task RemoveAsync(Category category, CancellationToken ct = default);
        Task<List<Category>> ListAsync(CancellationToken ct = default);
    }

    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
