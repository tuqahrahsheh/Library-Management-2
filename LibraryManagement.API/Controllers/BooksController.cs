using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Persistence.Scaffolded;
using Entities = LibraryManagement.Infrastructure.Persistence.Scaffolded.Entities;
using System.Data;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContextScaffold _db;
        public BooksController(LibraryDbContextScaffold db) => _db = db;

        public record BookResponse(int Id, string Title, string Author, string? ISBN, DateTime? PublishedDate, int Quantity, List<int> CategoryIds);
        public record CreateBookRequest(string Title, string Author, string? ISBN, DateTime? PublishedDate, List<int> CategoryIds);
        public record UpdateBookRequest(int Id, string Title, string Author, string? ISBN, DateTime? PublishedDate, List<int> CategoryIds);

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<List<BookResponse>>> List()
        {
            var books = await _db.Books
                .Include(b => b.Categories)
                .OrderBy(b => b.Title)
                .ToListAsync();

            var result = books.Select(b => new BookResponse(
                b.Id, b.Title, b.Author, b.Isbn, b.PublishedDate, b.Quantity,
                b.Categories.Select(c => c.Id).ToList()
            )).ToList();

            return Ok(result);
        }

        // GET: api/books/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookResponse>> Get(int id)
        {
            var b = await _db.Books
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (b is null) return NotFound();

            return Ok(new BookResponse(
                b.Id, b.Title, b.Author, b.Isbn, b.PublishedDate, b.Quantity,
                b.Categories.Select(c => c.Id).ToList()
            ));
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookResponse>> Create([FromBody] CreateBookRequest req)
        {
            if (req.CategoryIds is null || req.CategoryIds.Count == 0)
                return BadRequest("At least one categoryId is required.");

            var cats = await _db.Categories
                .Where(c => req.CategoryIds.Contains(c.Id))
                .ToListAsync();

            if (cats.Count == 0)
                return BadRequest("Provided categoryIds were not found.");

            var book = new Entities.Book
            {
                Title = req.Title,
                Author = req.Author,
                Isbn = req.ISBN,
                PublishedDate = req.PublishedDate,
                Quantity = 0,
                
                CategoryId = cats.First().Id
            };

            foreach (var c in cats)
                book.Categories.Add(c);

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            var dto = new BookResponse(
                book.Id, book.Title, book.Author, book.Isbn, book.PublishedDate, book.Quantity,
                book.Categories.Select(c => c.Id).ToList()
            );

            return CreatedAtAction(nameof(Get), new { id = book.Id }, dto);
        }

        // PUT: api/books/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookRequest req)
        {
            if (id != req.Id) return BadRequest();

            var book = await _db.Books
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book is null) return NotFound();

            var cats = await _db.Categories
                .Where(c => req.CategoryIds.Contains(c.Id))
                .ToListAsync();

            if (cats.Count == 0)
                return BadRequest("Provided categoryIds were not found.");

            book.Title = req.Title;
            book.Author = req.Author;
            book.Isbn = req.ISBN;
            book.PublishedDate = req.PublishedDate;

           
            book.Categories.Clear();
            foreach (var c in cats)
                book.Categories.Add(c);

            
            book.CategoryId = cats.First().Id;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/books/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book is null) return NotFound();

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("with-categories-sp")]
        public async Task<ActionResult<List<BookResponse>>> ListWithCategoriesViaSp()
        {
            var dict = new Dictionary<int, BookResponse>();

            var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.GetAllBooksWithCategories";
            cmd.CommandType = CommandType.StoredProcedure;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int bookId = reader.GetInt32(reader.GetOrdinal("BookId"));

                if (!dict.TryGetValue(bookId, out var dto))
                {
                    dto = new BookResponse(
                        Id: bookId,
                        Title: reader.GetString(reader.GetOrdinal("Title")),
                        Author: reader.GetString(reader.GetOrdinal("Author")),
                        ISBN: reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                        PublishedDate: reader.IsDBNull(reader.GetOrdinal("PublishedDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PublishedDate")),
                        Quantity: reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity")),
                        CategoryIds: new List<int>()
                    );
                    dict.Add(bookId, dto);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("CategoryId")))
                {
                    int catId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                    if (!dto.CategoryIds.Contains(catId)) dto.CategoryIds.Add(catId);
                }
            }

            return Ok(dict.Values.OrderBy(x => x.Title).ToList());
        }

    }
}
