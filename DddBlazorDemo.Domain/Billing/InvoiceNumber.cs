using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed record InvoiceNumber
{
    public string Value { get; }

    public InvoiceNumber(string value)
    {
        value = (value ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Invoice number is required.");
        if (value.Length > 32) throw new DomainException("Invoice number is too long.");
        Value = value;
    }

    public override string ToString() => Value;
}
