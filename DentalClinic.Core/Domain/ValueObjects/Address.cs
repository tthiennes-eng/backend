namespace DentalClinic.Core.Domain.ValueObjects;

public record Address
{
    public string Street { get; }
    public string Number { get; }
    public string? Complement { get; }
    public string Neighborhood { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }

    private Address(string street, string number, string? complement, string neighborhood, string city, string state, string zipCode)
    {
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
    }

    public static Address Create(string street, string number, string? complement, string neighborhood, string city, string state, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new InvalidOperationException("Street is required.");
        if (string.IsNullOrWhiteSpace(number)) throw new InvalidOperationException("Number is required.");
        if (string.IsNullOrWhiteSpace(neighborhood)) throw new InvalidOperationException("Neighborhood is required.");
        if (string.IsNullOrWhiteSpace(city)) throw new InvalidOperationException("City is required.");
        if (string.IsNullOrWhiteSpace(state)) throw new InvalidOperationException("State is required.");
        if (string.IsNullOrWhiteSpace(zipCode)) throw new InvalidOperationException("ZipCode is required.");

        return new Address(street, number, complement, neighborhood, city, state, zipCode);
    }
}
