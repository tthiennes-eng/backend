namespace DentalClinic.Core.Application.Interfaces;

/// <summary>
/// Interface para criptografia de senhas.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Gera o hash de uma senha.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifica se uma senha corresponde ao hash.
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}
