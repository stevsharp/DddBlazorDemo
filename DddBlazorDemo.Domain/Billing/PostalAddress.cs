using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed record PostalAddress
{
    public const int MaxStreetLength = 200;
    public const int MaxCityLength = 100;
    public const int MaxPostalCodeLength = 16;
    public const int CountryCodeLength = 2;

    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    /// <summary>ISO 3166-1 alpha-2, e.g. "GR", "DE".</summary>
    public string CountryCode { get; }

    public PostalAddress(string street, string city, string postalCode, string countryCode)
    {
        Street = Guard.MaxLength(Guard.NotBlank(street, "Street"), MaxStreetLength, "Street");
        City = Guard.MaxLength(Guard.NotBlank(city, "City"), MaxCityLength, "City");
        PostalCode = Guard.MaxLength(Guard.NotBlank(postalCode, "Postal code"), MaxPostalCodeLength, "Postal code");

        countryCode = Guard.NotBlank(countryCode, "Country").ToUpperInvariant();
        Guard.That(countryCode.Length == CountryCodeLength && countryCode.All(char.IsAsciiLetterUpper),
            "Country must be a 2-letter ISO code, e.g. GR.");
        CountryCode = countryCode;
    }

    public override string ToString() => $"{Street}, {PostalCode} {City}, {CountryCode}";
}
