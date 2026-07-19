using System.Security.Claims;
using DentalClinic.Core.Domain.Entities;

namespace DentalClinic.Core.Application.Interfaces;

/// <summary>
/// Interface para geração e validação de tokens JWT.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Gera um token de acesso (JWT) para o usuário.
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Gera um refresh token seguro.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Extrai os claims de um token expirado para validação do refresh token.
    /// </summary>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
