using DentalClinic.Core.Application.DTOs;
using DentalClinic.Core.Common;

namespace DentalClinic.Core.Application.Services;

/// <summary>
/// Interface para o serviço de autenticação e gestão de identidade.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica um usuário e gera os tokens de acesso.
    /// </summary>
    Task<Result<TokenDto>> AuthenticateAsync(LoginDto loginDto);

    /// <summary>
    /// Renova o token de acesso utilizando um refresh token válido.
    /// </summary>
    Task<Result<TokenDto>> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Revoga o token de um usuário (Logout).
    /// </summary>
    Task RevokeTokenAsync(string userId);
}
