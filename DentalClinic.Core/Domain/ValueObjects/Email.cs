using System.Text.RegularExpressions;

namespace DentalClinic.Core.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Email cannot be empty.");

        value = value.Trim().ToLower();

        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new InvalidOperationException("Invalid email format.");

        return new Email(value);
    }

    public override string ToString() => Value;
}
