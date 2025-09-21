-- 001_tables.sql
-- Create Tables (Books, Categories, BookCategories)
IF OBJECT_ID('dbo.BookCategories', 'U') IS NOT NULL DROP TABLE dbo.BookCategories;
IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL DROP TABLE dbo.Books;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
GO

CREATE TABLE dbo.Books (
  Id            INT IDENTITY(1,1) PRIMARY KEY,
  Title         NVARCHAR(200) NOT NULL,
  Author        NVARCHAR(150) NOT NULL,
  ISBN          NVARCHAR(50)  NULL,
  Description   NVARCHAR(MAX) NULL,
  Price         DECIMAL(10,2) NULL,
  PublishedDate DATE         NULL,
  Quantity      INT          NOT NULL DEFAULT 1
);
GO
CREATE INDEX IX_Books_Title  ON dbo.Books(Title);
CREATE INDEX IX_Books_Author ON dbo.Books(Author);
GO

CREATE TABLE dbo.Categories (
  Id   INT IDENTITY(1,1) PRIMARY KEY,
  Name NVARCHAR(100) NOT NULL
);
GO
CREATE UNIQUE INDEX IX_Categories_Name ON dbo.Categories(Name);
GO

CREATE TABLE dbo.BookCategories (
  BookId     INT NOT NULL,
  CategoryId INT NOT NULL,
  CONSTRAINT PK_BookCategories PRIMARY KEY (BookId, CategoryId),
  CONSTRAINT FK_BookCategories_Books
    FOREIGN KEY (BookId) REFERENCES dbo.Books(Id) ON DELETE CASCADE,
  CONSTRAINT FK_BookCategories_Categories
    FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id) ON DELETE CASCADE
);
GO
