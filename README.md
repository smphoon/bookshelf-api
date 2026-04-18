# Bookshelf API

A RESTful Web API built with ASP.NET Core (.NET 10) and Dapper, backed by Azure SQL Database. Built as part of a technical exercise demonstrating Azure-based architecture and API design.

## Tech Stack

- **Framework:** ASP.NET Core (.NET 10) - Controller-based
- **Data Access:** Dapper 
- **Database:** Azure SQL Database (free tier)
- **Documentation:** Swagger / OpenAPI
- **Hosting:** Azure App Service (F1 free tier)

## Architecture

The API follows a layered architecture pattern:
- **Controller** — handles HTTP concerns only (routing, model binding, response codes)
- **Service** — business logic and input guards
- **Repository** — all SQL queries via Dapper, no logic
- **Models** — separated into Entity, Request, and QueryParams to avoid over-posting

## Endpoints

### POST `/api/books`
Accepts a new book entry in **JSON** format.

### POST `/api/books/form`
Accepts a new book entry in **form-data** format.

### POST `/api/books/query`
Accepts a new book entry via **query string**.

### GET `/api/books`
Returns a paginated, filtered and sorted list of books.

| Parameter       | Type   | Default         | Description               |
|-----------------|--------|-----------------|---------------------------|
| `search`        | string | null            | Search term (min 3 chars) |
| `searchMode`    | string | contains        | `contains` or `equals`    |
| `sortBy`        | string | CreatedDateTime | Column to sort by         |
| `sortDirection` | string | desc            | `asc` or `desc`           |
| `page`          | int    | 1               | Page number               |
| `pageSize`      | int    | 5               | Records per page (max 50) |

## Design Decisions

**Dapper over Entity Framework Core**
Raw SQL queries are fully transparent and easy to explain. EF Core's generated SQL can be opaque during a code walkthrough. 

**Separate POST endpoints per input format**
`[FromBody]`, `[FromForm]`, and `[FromQuery]` cannot coexist on a single action in ASP.NET Core. Separate routes make the contract explicit and each endpoint has one clear responsibility.

**Sort column whitelist**
Column names cannot be parameterised in SQL, making ORDER BY a common injection vector. Valid sort columns are whitelisted explicitly in the repository before being interpolated into the query.

**OUTPUT clause on INSERT**
Returns the full inserted row including database-generated `Id` and audit timestamps in a single round trip, avoiding a separate SELECT after INSERT.

**No index on Title**
The SPA performs `LIKE '%keyword%'` contains searches. A nonclustered index on Title cannot be used by SQL Server for leading-wildcard LIKE queries, it would still result in a full scan. This is negligible at current demo scale. 

**Credentials**
The connection string is stored in 'appsettings.Development.json' which is gitignored. 

## Local Development

### Prerequisites
- .NET 10 SDK
- VS Code with C# Dev Kit and REST Client extensions
- Access to an Azure SQL Database instance

### Setup
1. Clone the repository
2. Create `appsettings.Development.json` in the project folder:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-azure-sql-connection-string"
  }
}
```
3. Run with `dotnet run` — Swagger UI available at `https://localhost:{port}/swagger`
4. Use `requests.http` to test all three input formats (JSON, form-data, query string)

> `appsettings.Development.json` is gitignored — never commit credentials to source control.

