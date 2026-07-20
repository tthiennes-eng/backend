using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DentalClinic.Core.Domain.Entities;
using DentalClinic.Core.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DentalClinic.Core.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<(string accessToken, string refreshToken, UserDto userDto)?> AuthenticateAsync(string email, string password)
        {
            // 1. Buscar usuário por Email (a propriedade agora é "Email", não "EmailAddress")
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
            {
                return null; // Usuário não encontrado
            }

            // 2. Verificar se o usuário está ativo (Status 0 = Active)
            if (user.Status != 0) 
            {
                return null; // Usuário bloqueado ou inativo
            }

            // 3. Validar a senha usando BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // Incrementar tentativas falhas (lógica simplificada)
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.Status = 1; // Bloquear
                    user.BlockedAt = DateTime.UtcNow;
                }
                await _userRepository.UpdateAsync(user);
                return null; // Senha incorreta
            }
            }

            // 4. Login bem sucedido: Resetar contadores e atualizar data
            user.FailedLoginAttempts = 0;
            user.LastLoginAt = DateTime.UtcNow;
            user.BlockedAt = null;
            // Se estava bloqueado mas a senha estava certa (caso raro de desbloqueio manual), mantém status
            // Mas se o status foi definido como bloqueado acima, ele permanece. 
            // Normalmente, se chegou aqui, o status já foi checado no passo 2.
            
            await _userRepository.UpdateAsync(user);

            // 5. Gerar Tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            // 6. Criar DTO de retorno
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email, // Usando a propriedade correta
                Role = user.Role    // Usando a propriedade correta
            };
            
            return (accessToken, refreshToken, userDto);
        }

        public async Task<bool> RefreshTokenAsync(string refreshToken, string currentAccessToken)
        {
            // Implementação simplificada: Em produção, valide o refresh token no banco
            // Aqui apenas verificamos se o token atual é válido para extrair o usuário
            // Para um exemplo funcional imediato, retornaremos true se o token atual for válido.
            
            try 
            {
                var principal = GetPrincipalFromExpiredToken(currentAccessToken);
                if (principal == null) return false;

                var email = principal.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email)) return false;

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null || user.Status != 0) return false;

                // Aqui você validaria o refreshToken contra o banco de dados
                // Por enquanto, assumimos que é válido para fins de teste
                
                return true;
            }
            catch 
            {
                return false;
            }
        }

        private string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Role", user.Role), // Claim personalizada para o Role
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expirationMinutes = _configuration.GetValue<int>("Jwt:AccessTokenExpirationMinutes");
            
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
        }
        
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false, // Importante: ignorar expiração para refresh
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            
            try 
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
                return principal;
            }
            catch 
            {
                return null;
            }
        }
    }
}