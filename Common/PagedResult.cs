namespace BookshelfApi.Common;
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }         // Total matching records
    public int Page { get; set; }               // Current page number
    public int PageSize { get; set; }           // Records per page
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}