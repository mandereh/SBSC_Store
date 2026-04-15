namespace Shared.DataTransferObjects;

public record ProductDto(Guid Id, string Name, decimal Price, string ImageUrl,  string Description);