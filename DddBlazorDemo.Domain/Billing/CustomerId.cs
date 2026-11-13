using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

/// <summary>
/// Strongly-typed identity. Prevents passing an invoice id where a customer id is expected;
/// the compiler catches it instead of the database.
/// </summary>
public readonly record struct CustomerId
{
    public Guid Value { get; }

    public CustomerId(Guid value) => Value = Guard.NotEmpty(value, "Customer id");

    public static CustomerId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
