using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

/// <summary>
/// Snapshot of the customer's billing details at the moment the invoice was created.
/// Owned by <see cref="Invoice"/>; does not change when the customer is edited later.
/// </summary>
public sealed record BillTo
{
    public CustomerName Name { get; }
    public VatNumber VatNumber { get; }
    public PostalAddress Address { get; }

    // EF Core only: it cannot pass the owned Address navigation to the public constructor,
    // so it needs a parameterless one and sets the properties through their backing fields.
    private BillTo()
    {
        Name = null!;
        VatNumber = null!;
        Address = null!;
    }

    public BillTo(CustomerName name, VatNumber vatNumber, PostalAddress address)
    {
        Name = Guard.NotNull(name, "Customer name");
        VatNumber = Guard.NotNull(vatNumber, "VAT number");
        Address = Guard.NotNull(address, "Address");
    }
}
