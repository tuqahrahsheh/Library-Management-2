using LibraryManagement.Application.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LibraryManagement.Infrastructure
{
    public sealed class BookReadRepositoryADO
    {
        private readonly string _connString;
        public BookReadRepositoryADO(IConfiguration cfg) => _connString = cfg.GetConnectionString("Default")!;

        public async Task<List<BookDto>> GetAllBooksWithCategoriesAsync(CancellationToken ct = default)
        {
            var dict = new Dictionary<int, BookDto>();
            await using var conn = new SqlConnection(_connString);
            await conn.OpenAsync(ct);

            await using var cmd = new SqlCommand("dbo.GetAllBooksWithCategories", conn)
            { CommandType = CommandType.StoredProcedure };

            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var bookId = reader.GetInt32(reader.GetOrdinal("BookId"));
                if (!dict.TryGetValue(bookId, out var dto))
                {
                    dto = new BookDto(
                        bookId,
                        reader.GetString(reader.GetOrdinal("Title")),
                        reader.GetString(reader.GetOrdinal("Author")),
                        reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                        reader.IsDBNull(reader.GetOrdinal("PublishedDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PublishedDate")),
                        reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity")),
                        new List<CategoryDto>()
                    );
                    dict.Add(bookId, dto);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("CategoryId")))
                {
                    dto.Categories.Add(new CategoryDto(
                        reader.GetInt32(reader.GetOrdinal("CategoryId")),
                        reader.GetString(reader.GetOrdinal("CategoryName"))
                    ));
                }
            }
            return dict.Values.ToList();
        }
    }
}
