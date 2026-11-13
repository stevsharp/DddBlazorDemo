using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed record CustomerName
{
    public const int MaxLength = 200;

    public string Value { get; }

    public CustomerName(string value)
    {
        value = Guard.NotBlank(value, "Customer name");
        Guard.MaxLength(value, MaxLength, "Customer name");
        Value = value;
    }

    public override string ToString() => Value;
}
