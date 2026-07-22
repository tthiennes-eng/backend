using DentalClinic.Core.Application.DTOs;
using DentalClinic.Core.Application.Interfaces;
using DentalClinic.Core.Common;
using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace DentalClinic.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSessionRepository _sessionRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUserSessionRepository sessionRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<TokenDto>> AuthenticateAsync(LoginDto loginDto)
    {
        _logger.LogInformation(">>> Tentativa de login: {Email}", loginDto.Email);

        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null)
        {
            _logger.LogWarning(">>> Falha: Usuário não encontrado.");
            return Result<TokenDto>.Failure("Credenciais inválidas.");
        }

        if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            _logger.LogWarning(">>> Falha: Senha incorreta.");
            user.IncrementFailedLogin();
            await _userRepository.UpdateAsync(user);
            return Result<TokenDto>.Failure("Credenciais inválidas.");
        }

        if (!user.IsActive) // Sincronizado: IsActive em vez de Status
        {
            _logger.LogWarning(">>> Falha: Conta inativa.");
            return Result<TokenDto>.Failure("Sua conta está bloqueada.");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var session = UserSession.Create(user.Id, refreshToken, 7, "127.0.0.1");
        await _sessionRepository.AddAsync(session);

        user.UpdateLoginInfo();
        user.ResetFailedLogin();
        await _userRepository.UpdateAsync(user);

        var userDto = new UserDto(user.Id, user.Name, user.EmailAddress.Value, user.Role.ToString().ToLower());
        return Result<TokenDto>.Ok(new TokenDto(accessToken, refreshToken, userDto));
    }

    public async Task<Result<TokenDto>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var session = await _sessionRepository.GetByTokenAsync(request.RefreshToken);

        if (session == null || !session.IsActive)
            return Result<TokenDto>.Failure("Sessão inválida.");

        var user = await _userRepository.GetByIdAsync(session.UserId);
        if (user == null || !user.IsActive) // Sincronizado
            return Result<TokenDto>.Failure("Usuário inválido.");

        session.Revoke();
        await _sessionRepository.UpdateAsync(session);

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var newSession = UserSession.Create(user.Id, newRefreshToken, 7, session.CreatedByIp);
        await _sessionRepository.AddAsync(newSession);

        var userDto = new UserDto(user.Id, user.Name, user.EmailAddress.Value, user.Role.ToString().ToLower());
        return Result<TokenDto>.Ok(new TokenDto(newAccessToken, newRefreshToken, userDto));
    }

    public async Task RevokeTokenAsync(string userId)
    {
        if (Guid.TryParse(userId, out var userGuid))
        {
            await _sessionRepository.RevokeAllUserSessionsAsync(userGuid);
        }
    }
}
