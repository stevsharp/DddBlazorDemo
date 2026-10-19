using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

/// <summary>
/// A GS1 identifier (EAN-8, UPC-A, EAN-13/GLN or GTIN-14): digits only with a valid mod-10 check digit.
/// </summary>
public sealed record EanNumber
{
    private static readonly int[] AllowedLengths = [8, 12, 13, 14];

    public string Value { get; }

    public EanNumber(string value)
    {
        value = Guard.NotBlank(value, "EAN number");

        Guard.That(value.All(char.IsAsciiDigit), "EAN number must contain only digits.");
        Guard.That(AllowedLengths.Contains(value.Length), "EAN number must be 8, 12, 13 or 14 digits long.");
        Guard.That(HasValidCheckDigit(value), "EAN number has an invalid check digit.");

        Value = value;
    }

    /// <summary>GS1 mod-10: weights 3 and 1 alternate, starting with 3 at the rightmost payload digit.</summary>
    private static bool HasValidCheckDigit(string digits)
    {
        var sum = 0;
        var weight = 3;

        for (var i = digits.Length - 2; i >= 0; i--)
        {
            sum += (digits[i] - '0') * weight;
            weight = weight == 3 ? 1 : 3;
        }

        var expected = (10 - sum % 10) % 10;
        return digits[^1] - '0' == expected;
    }

    public override string ToString() => Value;
}
