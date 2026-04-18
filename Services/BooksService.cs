using BookshelfApi.Common;
using BookshelfApi.Models;
using BookshelfApi.Repositories;

namespace BookshelfApi.Services;

public class BooksService : IBooksService
{
    private readonly IBooksRepository _repository;

    public BooksService(IBooksRepository repository)
    {
        _repository = repository;
    }

    public Task<PagedResult<Book>> GetBooksAsync(BookQueryParams queryParams)
    {
        // Enforce minimum page size guard to prevents client requesting 10000 rows/call, exhausting free tier compute
        queryParams.PageSize = Math.Clamp(queryParams.PageSize, 1, 50);
        queryParams.Page = Math.Max(queryParams.Page, 1);

        return _repository.GetBooksAsync(queryParams);
    }

    public Task<Book> CreateAsync(BookRequest request)
    {
        // keeping this thin for demo scope
        // can expand business rules like duplicate title detection, status workflow validation later.
        return _repository.CreateAsync(request);
    }
}