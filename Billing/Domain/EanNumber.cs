using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed record EanNumber
{
    public string Value { get; }

    public EanNumber(string value)
    {
        value = (value ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("EAN number is required.");
        if (value.Length is < 8 or > 20) throw new DomainException("EAN number length looks invalid.");
        Value = value;
    }

    public override string ToString() => Value;
}
