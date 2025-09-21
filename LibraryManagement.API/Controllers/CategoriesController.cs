using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Persistence.Scaffolded;
using S = LibraryManagement.Infrastructure.Persistence.Scaffolded.Entities;



namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly LibraryDbContextScaffold _db;
        public CategoriesController(LibraryDbContextScaffold db) => _db = db;

        public class CreateCategoryRequest
        {
            [Required, MinLength(1)]
            public string Name { get; set; } = string.Empty;
        }

        public class UpdateCategoryRequest
        {
            [Required]
            public int Id { get; set; }

            [Required, MinLength(1)]
            public string Name { get; set; } = string.Empty;
        }

        public record CategoryResponse(int Id, string Name);

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryResponse>>> List()
        {
            var cats = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
            return Ok(cats.Select(c => new CategoryResponse(c.Id, c.Name)).ToList());
        }

        // GET: api/categories/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> Get(int id)
        {
            var c = await _db.Categories.FindAsync(id);
            return c is null ? NotFound() : Ok(new CategoryResponse(c.Id, c.Name));
        }

        // POST: api/categories
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest req)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var c = new LibraryManagement.Infrastructure.Persistence.Scaffolded.Entities.Category { Name = req.Name.Trim() };

            _db.Categories.Add(c);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = c.Id }, new CategoryResponse(c.Id, c.Name));
        }

        // PUT: api/categories/5
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest req)
        {
            if (id != req.Id) return BadRequest("Path id must match body id.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var c = await _db.Categories.FindAsync(id);
            if (c is null) return NotFound();

            c.Name = req.Name.Trim();
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Categories.FindAsync(id);
            if (c is null) return NotFound();

            _db.Categories.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
