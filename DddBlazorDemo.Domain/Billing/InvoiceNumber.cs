using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed record InvoiceNumber
{
    public const int MaxLength = 32;

    public string Value { get; }

    public InvoiceNumber(string value)
    {
        value = Guard.NotBlank(value, "Invoice number");
        Guard.MaxLength(value, MaxLength, "Invoice number");
        Guard.That(value.All(IsAllowedChar), "Invoice number may contain only letters, digits, '-', '_', '/' and '.'.");

        Value = value;
    }

    private static bool IsAllowedChar(char c)
        => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '/' or '.';

    public override string ToString() => Value;
}
