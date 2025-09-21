-- 200_seed.sql (اختياري)
INSERT INTO dbo.Categories(Name) VALUES (N''Programming''), (N''Fiction'');

INSERT INTO dbo.Books(Title, Author, ISBN, PublishedDate, Quantity)
VALUES
 (N''C# in Depth'', N''Jon Skeet'', N''9781617294532'', ''2019-01-01'', 1),
 (N''Learning Angular'', N''Brad Dayley'', N''9780137355679'', ''2021-01-01'', 1),
 (N''Pro ASP.NET Core MVC'', N''Adam Freeman'', N''9781484254394'', ''2020-01-01'', 1);

INSERT INTO dbo.BookCategories(BookId, CategoryId) VALUES
 (1,1),
 (2,1),
 (3,1);
