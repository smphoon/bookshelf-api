using BookshelfApi.Common;
using BookshelfApi.Models;

namespace BookshelfApi.Services;

public interface IBooksService
{
    Task<PagedResult<Book>> GetBooksAsync(BookQueryParams queryParams);
    Task<Book> CreateAsync(BookRequest request);
}