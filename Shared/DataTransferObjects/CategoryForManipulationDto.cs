using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public abstract record CategoryForManipulationDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; init; }
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; init; }
    IEnumerable<ProductForUpdateDto>? Product { get; init; }
}