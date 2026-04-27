using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record ReviewForCreationDto
{
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; init; }
    
    [Required(ErrorMessage = "Comment is required")]
    [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
    public string? Comment { get; init; }
};