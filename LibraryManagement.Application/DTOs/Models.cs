namespace LibraryManagement.Application.DTOs
{
    public record CategoryDto(int Id, string Name);
    public record BookDto(int Id, string Title, string Author, string? ISBN, DateTime? PublishedDate, int Quantity, List<CategoryDto> Categories);

    public record CreateBookRequest(string Title, string Author, string? ISBN, DateTime? PublishedDate, int Quantity, List<int> CategoryIds);
    public record UpdateBookRequest(int Id, string Title, string Author, string? ISBN, DateTime? PublishedDate, int Quantity, List<int> CategoryIds);

    public record CreateCategoryRequest(string Name, string? Description);
    public record UpdateCategoryRequest(int Id, string Name, string? Description);
}
