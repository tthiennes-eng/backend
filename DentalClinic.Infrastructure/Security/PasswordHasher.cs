using DentalClinic.Core.Application.Interfaces;
using BCrypt.Net;

namespace DentalClinic.Infrastructure.Security;

/// <summary>
/// Implementação de hashing de senhas utilizando BCrypt com WorkFactor 12.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
