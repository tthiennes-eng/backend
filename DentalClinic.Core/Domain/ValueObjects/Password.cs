namespace DentalClinic.Core.Domain.ValueObjects;

/// <summary>
/// Value Object que representa uma senha validada.
/// Armazena apenas a senha em texto plano para validação inicial.
/// O hash será responsabilidade da camada de infraestrutura.
/// </summary>
public sealed class Password : ValueObject
{
    private const int MinimumLength = 8;
    private const int MaximumLength = 128;

    /// <summary>
    /// Senha em texto plano (deve ser hasheada antes do armazenamento).
    /// </summary>
    public string Value { get; private set; }

    private Password() => Value = string.Empty;

    /// <summary>
    /// Cria um novo Value Object Password.
    /// </summary>
    /// <param name="password">Senha em texto plano.</param>
    /// <returns>Um novo Password ou lança InvalidOperationException se inválido.</returns>
    /// <exception cref="InvalidOperationException">Lançado quando a senha não atende aos requisitos.</exception>
    public static Password Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Senha não pode ser vazia.");

        if (password.Length < MinimumLength)
            throw new InvalidOperationException($"Senha deve ter no mínimo {MinimumLength} caracteres.");

        if (password.Length > MaximumLength)
            throw new InvalidOperationException($"Senha não pode ter mais de {MaximumLength} caracteres.");

        if (!HasUpperCase(password))
            throw new InvalidOperationException("Senha deve conter pelo menos uma letra maiúscula.");

        if (!HasLowerCase(password))
            throw new InvalidOperationException("Senha deve conter pelo menos uma letra minúscula.");

        if (!HasDigit(password))
            throw new InvalidOperationException("Senha deve conter pelo menos um número.");

        if (!HasSpecialCharacter(password))
            throw new InvalidOperationException("Senha deve conter pelo menos um caractere especial (!@#$%^&*).");

        return new Password { Value = password };
    }

    private static bool HasUpperCase(string password) => password.Any(char.IsUpper);
    private static bool HasLowerCase(string password) => password.Any(char.IsLower);
    private static bool HasDigit(string password) => password.Any(char.IsDigit);
    private static bool HasSpecialCharacter(string password) => password.Any(c => "!@#$%^&*".Contains(c));

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}
