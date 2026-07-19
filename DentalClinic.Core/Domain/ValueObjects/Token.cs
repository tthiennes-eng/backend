namespace DentalClinic.Core.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um token de autenticação.
/// </summary>
public sealed class Token : ValueObject
{
    /// <summary>
    /// O valor do token.
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Data e hora de expiração do token.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    private Token()
    {
        Value = string.Empty;
    }

    /// <summary>
    /// Cria um novo Value Object Token.
    /// </summary>
    /// <param name="value">Valor do token.</param>
    /// <param name="expiresAt">Data e hora de expiração.</param>
    /// <returns>Um novo Token ou lança InvalidOperationException se inválido.</returns>
    /// <exception cref="InvalidOperationException">Lançado quando o token ou expiração são inválidos.</exception>
    public static Token Create(string value, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Valor do token não pode ser vazio.");

        if (expiresAt <= DateTime.UtcNow)
            throw new InvalidOperationException("Token já expirou ou a data de expiração é inválida.");

        return new Token
        {
            Value = value,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Verifica se o token expirou.
    /// </summary>
    /// <returns>Verdadeiro se o token expirou, falso caso contrário.</returns>
    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;

    /// <summary>
    /// Calcula quantos segundos faltam para expiração.
    /// </summary>
    /// <returns>Número de segundos até expiração.</returns>
    public int SecondsUntilExpiration => (int)Math.Max(0, (ExpiresAt - DateTime.UtcNow).TotalSeconds);

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
        yield return ExpiresAt;
    }

    public override string ToString() => Value;
}
