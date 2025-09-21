Library Management System 
(Backend: .NET Core API, Frontend: Angular)
Onion Architecture DDD Repository Pattern EF Core ADO.NET + Stored Procedure 
Swagger

Deliverables
Source Code in a GitHub repository (API + Angular).
Database Scripts (tables and stored procedures) under /sql.
README including setup, time estimation vs. actual, challenges, and AI tool usage with prompts & notes.
Repository: https://github.com/tuqahrahsheh/Library-Management-2

Architecture (Onion + DDD + Repository Pattern)
Domain: core entities.
Application: DTOs, AutoMapper mapping profile(s), repository abstractions.
Infrastructure: EF Core (DbContext + repositories), ADO.NET (SqlClient) for stored-procedure read, SQL scripts.
API: Web API (controllers, Swagger/OpenAPI, CORS).

Repository Layout
/ (root)
LibraryManagement.sln
LibraryManagement.API/
LibraryManagement.Application/
LibraryManagement.Domain/
LibraryManagement.Infrastructure/
frontend/ Angular app
sql/
 001_tables.sql creates Books, Categories, BookCategories
 100_sp_GetAllBooksWithCategories.sql
 200_seed.sql optional seed data


Database Setup
Open SSMS and connect to your SQL Server instance (e.g., DESKTOP-42TED2T).
Ensure database exists (LibraryDB). If not: CREATE DATABASE LibraryDB;
Execute scripts in order: 001_tables.sql 100_sp_GetAllBooksWithCategories.sql 200_seed.sql (optional).
Quick check: SELECT counts and EXEC dbo.GetAllBooksWithCategories.

DB Quick Check
USE LibraryDB;
GO
SELECT COUNT(*) AS Books FROM dbo.Books;
SELECT COUNT(*) AS Cats  FROM dbo.Categories;
SELECT COUNT(*) AS Links FROM dbo.BookCategories;
EXEC dbo.GetAllBooksWithCategories;


API Setup & Run
Update connection string in LibraryManagement.API/appsettings.json:
{
  "ConnectionStrings": {
    "Default": "Server=DESKTOP-42TED2T;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}


Run Commands
From the repository root:
dotnet restore
dotnet build
dotnet run --project LibraryManagement.API
Swagger UI is available at the root (RoutePrefix = ""). CORS enabled for http://localhost:4200 and http://localhost:4300.

Key Endpoints
Books (EF CRUD):
  GET /api/books
  GET /api/books/{id}
  POST /api/books
  PUT /api/books
  DELETE /api/books/{id}
Books via ADO.NET + Stored Procedure:
  GET /api/books/with-categories-sp   (reads from dbo.GetAllBooksWithCategories)
Categories (EF CRUD):
  GET /api/categories
  POST /api/categories
  PUT /api/categories
  DELETE /api/categories/{id}

Frontend (Angular) Setup & Run
Open a terminal:
cd frontend
npm i
ng serve --port 4300 -o
Set API base URL in frontend/src/environments/environment.ts:
export const environment = {
  apiBase: 'http://localhost:5265' // or https://localhost:7110 if enabled
};


Frontend Summary
Pages: Books and Categories (list, create, update, delete).
Books list uses the stored-procedure endpoint (/api/books/with-categories-sp).
Create/Update/Delete use EF CRUD endpoints.

Time Estimation vs Actual
Estimated: 1 week
Actual: 4 days
Breakdown:
  Day 1: Multi-project solution layout, database design decisions, NuGet alignment (AutoMapper), basic controllers.
  Day 2: EF Core scaffolding, repositories, ADO.NET + SP endpoint, Swagger and CORS configuration.
  Day 3: Angular Material setup; Categories & Books pages (CRUD) + SP-based read.
  Day 4: Stabilization, OneDrive conflicts cleanup, SQL scripts and documentation.

Challenges Faced
Schema alignment: migrating from a single-category column to a clean many-to-many via BookCategories.
AutoMapper version conflicts (NU1107): resolved by pinning compatible versions across projects.
Angular setup on Windows (ExecutionPolicy + @angular/animations) and ensuring CORS between API and UI ports.
Exposing Swagger at root to simplify review (RoutePrefix = "").
OneDrive interference during file edits: mitigated by pausing sync while building.

AI Tool Usage (Prompts + Summaries + My Notes)
I used AI assistants sparingly to speed up troubleshooting and cross-check patterns; I did not rely on them exclusively. Below are representative paraphrased prompts, short assistant summaries, and my own notes on what I changed or kept.

1) Onion + DDD layering
Prompt (paraphrased): Propose a clean .NET solution split (Domain, Application, Infrastructure, API) and where to place entities, DTOs, mapping, and repository abstractions.
Assistant summary: Suggested a four-project solution; Domain for entities; Application for DTOs/mapping/abstractions; Infrastructure for EF/ADO; API for controllers.
My notes & adjustments: Adopted the split; kept AutoMapper profile(s) and DTOs in Application; repository implementations in Infrastructure.

2) AutoMapper NU1107 version conflict
Prompt (paraphrased): AutoMapper.Extensions.Microsoft.DependencyInjection requires AutoMapper 12 while the project references 15. How to align?
Assistant summary: Use DI package compatible with v15 or downgrade AutoMapper to v12 across projects.
My notes & adjustments: Pinned compatible versions across projects; ensured consistent restore/build without conflicts.

3) Stored Procedure + ADO.NET reader
Prompt (paraphrased): Write a stored procedure to return books with categories (many-to-many) and an ADO.NET reader that groups results per book.
Assistant summary: LEFT JOINs with a bridge table; accumulate rows in a dictionary keyed by BookId; handle DBNull.
My notes & adjustments: Implemented the approach; added explicit DBNull checks and stable ordering of results.

4) Swagger at root and enabling CORS
Prompt (paraphrased): How to expose Swagger at '/' and allow Angular dev ports (4200/4300)?
Assistant summary: Use AddCors/UseCors with allowed origins; UseSwagger/UseSwaggerUI with RoutePrefix = "".
My notes & adjustments: Applied exactly; verified both HTTP and HTTPS dev urls.

5) Angular setup issues on Windows
Prompt (paraphrased): Fix execution policy and missing @angular/animations error when serving the app.
Assistant summary: Set-ExecutionPolicy RemoteSigned; install @angular/animations with matching major version; restart dev server.
My notes & adjustments: Followed the steps; confirmed ng serve worked and Material components loaded.

6) EF Core scaffold for selected tables
Prompt (paraphrased): Command to scaffold DbContext for specific tables and output to a custom folder.
Assistant summary: Provided dotnet ef dbcontext scaffold with --table and --output-dir switches.
My notes & adjustments: Used targeted scaffolding; retained hand-authored DbContext where appropriate.

7) Safely dropping legacy columns/constraints
Prompt (paraphrased): How to drop a column like Books.CategoryId that may have FKs/Defaults/Indexes attached?
Assistant summary: Script that locates and drops dependent constraints first, then drops the column.
My notes & adjustments: Applied selectively with checks in SSMS; verified clean drops before creating the M:N schema.

Manual Test Checklist
Swagger:
  POST /api/categories  { "name": "Programming" }
  POST /api/books       required fields + categoryIds [<id>]
  GET  /api/books/with-categories-sp
UI (Angular):
  Categories: add/edit/delete
  Books: add/edit/delete; list reads via SP endpoint

.gitignore (Important)
/**/bin/
/**/obj/
.vs/
/frontend/node_modules/
/frontend/dist/
/frontend/.angular/


Contact
Tuqa Hrahsheh
GitHub: https://github.com/tuqahrahsheh
Repository: https://github.com/tuqahrahsheh/Library-Management-2

Generated on: 2025-09-21 08:51