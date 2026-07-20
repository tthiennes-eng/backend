using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DentalClinic.Core.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        
        if (user == null || !user.IsActive)
        {
            return new AuthResult(false, "Usuário ou senha inválidos", null, null);
        }

        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            await _userRepository.IncrementFailedLoginAttemptsAsync(user.Id);
            
            if (user.FailedLoginAttempts >= 5)
            {
                await _userRepository.LockUserAsync(user.Id);
                return new AuthResult(false, "Conta bloqueada por múltiplas tentativas falhas", null, null);
            }
            
            return new AuthResult(false, "Usuário ou senha inválidos", null, null);
        }

        await _userRepository.ResetFailedLoginAttemptsAsync(user.Id);

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        await _userRepository.UpdateLastLoginAsync(user.Id);

        return new AuthResult(true, "Login realizado com sucesso", token, refreshToken);
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
        
        if (user == null || !user.IsActive)
        {
            return new AuthResult(false, "Token de atualização inválido", null, null);
        }

        var token = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        return new AuthResult(true, "Token atualizado com sucesso", token, newRefreshToken);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null || !_passwordHasher.Verify(currentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = _passwordHasher.Hash(newPassword);
        await _userRepository.UpdateAsync(user);
        
        return true;
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        
        if (user == null)
        {
            return false;
        }

        var resetToken = Guid.NewGuid().ToString();
        await _userRepository.SetPasswordResetTokenAsync(user.Id, resetToken, DateTime.UtcNow.AddHours(24));
        
        // Aqui você implementaria o envio do email com o token
        // await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
        
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var user = await _userRepository.GetUserByPasswordResetTokenAsync(token);
        
        if (user == null || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        user.PasswordHash = _passwordHasher.Hash(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;
        
        await _userRepository.UpdateAsync(user);
        
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("Roles", string.Join(",", user.Roles))
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
    }
}
