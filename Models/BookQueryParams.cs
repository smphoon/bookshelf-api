namespace BookshelfApi.Models;

public class BookQueryParams
{
    // Search
    public string? Search { get; set; } // Applied as LIKE '%search%'

    //SearchMode: equals || contains  
    public string SearchMode { get; set; } = "contains";    //default to contains

    // Sorting
    public string SortBy { get; set; } = "CreatedDateTime";
    public string SortDirection { get; set; } = "desc"; // asc || desc

    // Pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}