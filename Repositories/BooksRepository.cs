using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using BookshelfApi.Common;
using BookshelfApi.Models;

namespace BookshelfApi.Repositories;

public class BooksRepository : IBooksRepository
{
    private readonly string _connectionString;

    // Connection string injected via constructor
    public BooksRepository(IConfiguration configuration)
    {
        _connectionString = configuration
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
    }

    // Factory method: creates and disposes connection per operation
    private IDbConnection CreateConnection() =>
        new SqlConnection(_connectionString);

    // Retry wrapper for Azure free tier auto pause. Catch error 40613: database unavailable during resume
    private async Task<T> WithRetry<T>(Func<Task<T>> operation)
    {
        try
        {
            return await operation();
        }
        catch (SqlException ex) when (ex.Number == 40613)
        {
            // 40613 = database unavailable (auto-pause resuming)
            // Wait 30 seconds and retry once
            await Task.Delay(TimeSpan.FromSeconds(30));
            return await operation();
        }
    }

    public Task<PagedResult<Book>> GetBooksAsync(BookQueryParams queryParams) =>
        WithRetry(async () =>
        {
            // Whitelist valid sort columns to prevent SQL injection via sortBy
            var allowedSortColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Title", "Author", "Genre", "Status",
                "CreatedDateTime", "ModifiedDateTime"
            };

            var sortColumn = allowedSortColumns.Contains(queryParams.SortBy)
                ? queryParams.SortBy
                : "CreatedDateTime";

            var sortDirection = queryParams.SortDirection
                .Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? "ASC"
                : "DESC";

            var offset = (queryParams.Page - 1) * queryParams.PageSize;

            // Build WHERE clause based on search mode
            // use LIKE '% keywords %' for contains, use exact match for equals
            // Parameterized to prevent SQL Injection
            var isEquals = queryParams.SearchMode.Equals("equals", StringComparison.OrdinalIgnoreCase);

            var whereClause = isEquals ? 
                """
                (@Search IS NULL
                    OR Title = @Search
                    OR Author = @Search
                    OR Genre = @Search)
                """ : """
                (@Search IS NULL
                    OR Title LIKE '%' + @Search + '%'
                    OR Author LIKE '%' + @Search + '%'
                    OR Genre LIKE '%' + @Search + '%')
                """;

            var sql = $"""
                SELECT COUNT(*)
                FROM [dbo].[Books]
                WHERE {whereClause};

                SELECT Id, Title, Author, Genre, Notes,
                    Status, CreatedDateTime, ModifiedDateTime
                FROM [dbo].[Books]
                WHERE {whereClause}
                ORDER BY [{sortColumn}] {sortDirection}
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
                """;

            using var connection = CreateConnection();
            using var multi = await connection.QueryMultipleAsync(sql, new
            {
                Search = string.IsNullOrWhiteSpace(queryParams.Search)
                    ? null
                    : queryParams.Search,
                Offset = offset,
                PageSize = queryParams.PageSize
            });

            var totalCount = await multi.ReadSingleAsync<int>();
            var items = await multi.ReadAsync<Book>();

            return new PagedResult<Book>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        });

    public Task<Book?> GetByIdAsync(int id)  =>
        WithRetry(async () =>
        {
            var sql = """
                SELECT Id, Title, Author, Genre, Notes,
                    Status, CreatedDateTime, ModifiedDateTime
                FROM [dbo].[Books]
                WHERE Id = @Id;
                """;

            using var connection = CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Book>(sql, new { Id = id });
        });

    public Task<Book> CreateAsync(BookRequest request)  =>
        WithRetry(async () =>
        {
            // OUTPUT clause returns the full inserted row in a single round trip 
            // avoids separate SELECT after INSERT
            var sql = """
                INSERT INTO [dbo].[Books]
                    ([Title], [Author], [Genre], [Notes], [Status])
                OUTPUT
                    INSERTED.Id,
                    INSERTED.Title,
                    INSERTED.Author,
                    INSERTED.Genre,
                    INSERTED.Notes,
                    INSERTED.Status,
                    INSERTED.CreatedDateTime,
                    INSERTED.ModifiedDateTime
                VALUES
                    (@Title, @Author, @Genre, @Notes, @Status);
                """;

            using var connection = CreateConnection();
            return await connection.QuerySingleAsync<Book>(sql, new
            {
                request.Title,
                request.Author,
                request.Genre,
                request.Notes,
                request.Status
            });
        });
}