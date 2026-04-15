namespace Shared.DataTransferObjects;

public record CategoryDto(Guid Id, string Name, string Description, IEnumerable<ProductDto>? Products);