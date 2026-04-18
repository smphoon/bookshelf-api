using BookshelfApi.Common;
using BookshelfApi.Models;

namespace BookshelfApi.Repositories;

public interface IBooksRepository
{
    Task<PagedResult<Book>> GetBooksAsync(BookQueryParams queryParams);
    Task<Book?> GetByIdAsync(int id);
    Task<Book> CreateAsync(BookRequest request);
}