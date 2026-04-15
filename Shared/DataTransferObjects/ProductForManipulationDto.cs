using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public abstract record ProductForManipulationDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; init; }
    [Required(ErrorMessage = "Price is required")]
    public string Price { get; init; }
    public string imageUrl { get; init; }
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; init; }
}