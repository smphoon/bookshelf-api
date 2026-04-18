using Microsoft.AspNetCore.Mvc;
using BookshelfApi.Models;
using BookshelfApi.Services;

namespace BookshelfApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _service;

    public BooksController(IBooksService service)
    {
        _service = service;
    }

    // GET /api/books?search=dune&sortBy=Title&sortDirection=asc&page=1&pageSize=5
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] BookQueryParams queryParams)
    {
        var result = await _service.GetBooksAsync(queryParams);
        return Ok(result);
    }

    // POST /api/books
    // Accepts JSON, form-data, and query string
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] BookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetBooks), new { id = created.Id }, created);
    }

    // POST /api/books/form
    // Endpoint for form data submissions.
    [HttpPost("form")]
    public async Task<IActionResult> CreateBookForm([FromForm] BookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetBooks), new { id = created.Id }, created);
    }

    // POST /api/books/query
    // Endpoint for query string submission
    // e.g. POST /api/books/query?title=Dune&author=Herbert&genre=Sci-Fi
    [HttpPost("query")]
    public async Task<IActionResult> CreateBookQuery([FromQuery] BookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetBooks), new { id = created.Id }, created);
    }
}