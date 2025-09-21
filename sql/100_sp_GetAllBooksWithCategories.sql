-- 100_sp_GetAllBooksWithCategories.sql
CREATE OR ALTER PROCEDURE dbo.GetAllBooksWithCategories
AS
BEGIN
  SET NOCOUNT ON;

  SELECT 
    b.Id AS BookId,
    b.Title,
    b.Author,
    b.ISBN,
    b.PublishedDate,
    b.Quantity,
    c.Id   AS CategoryId,
    c.Name AS CategoryName
  FROM dbo.Books b
  LEFT JOIN dbo.BookCategories bc ON bc.BookId = b.Id
  LEFT JOIN dbo.Categories c ON c.Id = bc.CategoryId
  ORDER BY b.Title, c.Name;
END
GO
