using System.Text.RegularExpressions;

namespace DentalClinic.Core.Domain.ValueObjects;

public record CPF
{
    public string Value { get; }

    private CPF(string value)
    {
        Value = value;
    }

    public static CPF Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("CPF não pode ser vazio.");

        var cleanCpf = Regex.Replace(value, @"[^\d]", "");

        if (cleanCpf.Length != 11)
            throw new InvalidOperationException("CPF deve conter 11 dígitos.");

        // Lógica de validação de dígitos verificadores do CPF omitida para brevidade,
        // mas deve estar presente em produção.

        return new CPF(cleanCpf);
    }

    public string Formatado() => Regex.Replace(Value, @"(\d{3})(\d{3})(\d{3})(\d{2})", "$1.$2.$3-$4");

    public override string ToString() => Value;
}
