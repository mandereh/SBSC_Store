namespace Shared.DataTransferObjects;

public record RegistrationResponseDto(RegisteredUserDto User, TokenDto Token);

