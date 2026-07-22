namespace DentalClinic.Core.Domain.ValueObjects;

public record Address
{
    public string Street { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Complement { get; set; }
    public string Neighborhood { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    public Address() { }

    public Address(string street, string number, string? complement, string neighborhood, string city, string state, string zipCode)
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
        return new Address(street, number, complement, neighborhood, city, state, zipCode);
    }
}
