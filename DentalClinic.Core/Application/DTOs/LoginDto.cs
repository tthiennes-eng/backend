namespace DentalClinic.Core.Application.DTOs;

public record LoginDto(string Email, string Password);

public record TokenDto(string AccessToken, string RefreshToken, UserDto User);

public record UserDto(Guid Id, string Name, string Email, string Role);

public record RefreshTokenRequest(string RefreshToken);
