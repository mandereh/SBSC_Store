using System.ComponentModel.DataAnnotations;
namespace Shared.DataTransferObjects;

public abstract record ProductForManipulationDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; init; }
    [Required(ErrorMessage = "Price is required")]
    public string Price { get; init; }
    // Image file is handled at the controller level via multipart/form-data and passed separately to the service.
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; init; }
}