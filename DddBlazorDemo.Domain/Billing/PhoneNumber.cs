using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

/// <summary>Stored normalised: digits with an optional leading '+'. Spaces, dashes and parentheses are removed.</summary>
public sealed record PhoneNumber
{
    public const int MinDigits = 6;
    public const int MaxLength = 20;

    public string Value { get; }

    public PhoneNumber(string value)
    {
        value = Guard.NotBlank(value, "Phone number");
        value = new string(value.Where(c => char.IsDigit(c) || c == '+').ToArray());

        Guard.That(value.Count(char.IsDigit) >= MinDigits, $"Phone number must have at least {MinDigits} digits.");
        Guard.MaxLength(value, MaxLength, "Phone number");
        Guard.That(value.IndexOf('+') <= 0, "Phone number may only start with '+'.");

        Value = value;
    }

    public override string ToString() => Value;
}
