using System.ComponentModel.DataAnnotations;

namespace BookshelfApi.Models;

// This is what the API accepts on POST.
// Separate from Book entity as client should never control Id, CreatedDateTime or ModifiedDateTime
public class BookRequest
{
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Genre { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Want to Read";
}