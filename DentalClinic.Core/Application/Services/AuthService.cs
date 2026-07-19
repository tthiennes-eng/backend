using DentalClinic.Core.Application.DTOs;
using DentalClinic.Core.Application.Interfaces;
using DentalClinic.Core.Common;
using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace DentalClinic.Core.Application.Services;

/// <summary>
/// Implementação sênior do serviço de autenticação.
/// Gerencia login, revogação de sessões e renovação de tokens (Refresh Token).
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSessionRepository _sessionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IUserSessionRepository sessionRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<Result<TokenDto>> AuthenticateAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null || !user.CanAccess())
        {
            return Result<TokenDto>.Failure("Usuário não encontrado ou inativo.");
        }

        bool isPasswordValid = _passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            user.RegisterFailedLoginAttempt();
            await _userRepository.UpdateAsync(user);
            return Result<TokenDto>.Failure("Credenciais inválidas.");
        }

        user.RegisterSuccessfulLogin();
        await _userRepository.UpdateAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Persiste a sessão para controle de Refresh Token (Segurança LGPD)
        var expiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
        var session = UserSession.Create(user.Id, refreshToken, expiryDays, "0.0.0.0"); // Em produção, capturar IP real
        await _sessionRepository.AddAsync(session);

        var userDto = new UserDto(user.Id, user.Name, user.EmailAddress.Value, user.Roles.First().ToString());

        return Result<TokenDto>.Ok(new TokenDto(accessToken, refreshToken, userDto));
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var session = await _sessionRepository.GetByTokenAsync(request.RefreshToken);

        if (session == null || !session.IsActive)
        {
            return Result<TokenDto>.Failure("Sessão inválida ou expirada.");
        }

        var user = await _userRepository.GetByIdAsync(session.UserId);
        if (user == null || !user.CanAccess())
        {
            return Result<TokenDto>.Failure("Usuário não autorizado.");
        }

        // Revoga a sessão atual (Refresh Token Rotation para segurança extra)
        session.Revoke();
        await _sessionRepository.UpdateAsync(session);

        // Gera novo par de tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var expiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
        var newSession = UserSession.Create(user.Id, newRefreshToken, expiryDays, session.CreatedByIp);
        await _sessionRepository.AddAsync(newSession);

        var userDto = new UserDto(user.Id, user.Name, user.EmailAddress.Value, user.Roles.First().ToString());

        return Result<TokenDto>.Ok(new TokenDto(newAccessToken, newRefreshToken, userDto));
    }

    public async Task RevokeTokenAsync(string userId)
    {
        if (Guid.TryParse(userId, out var guid))
        {
            await _sessionRepository.RevokeAllUserSessionsAsync(guid);
        }
    }
}
