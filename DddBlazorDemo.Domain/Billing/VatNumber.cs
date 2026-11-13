using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

/// <summary>
/// Tax identification number. Stored normalised: upper-case, no spaces or dots
/// (e.g. "EL123456789", "DE123456789"). Format checks per country are out of scope here.
/// </summary>
public sealed record VatNumber
{
    public const int MinLength = 8;
    public const int MaxLength = 15;

    public string Value { get; }

    public VatNumber(string value)
    {
        value = Guard.NotBlank(value, "VAT number")
            .Replace(" ", "").Replace(".", "").Replace("-", "")
            .ToUpperInvariant();

        Guard.That(value.Length is >= MinLength and <= MaxLength,
            $"VAT number must be between {MinLength} and {MaxLength} characters.");
        Guard.That(value.All(char.IsAsciiLetterOrDigit),
            "VAT number may contain only letters and digits.");

        Value = value;
    }

    public override string ToString() => Value;
}
