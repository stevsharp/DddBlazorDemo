using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed class Customer : Entity, IAggregateRoot<CustomerId>
{
    private Customer() { }
    public CustomerId Id { get; private set; }
    public CustomerName Name { get; private set; } = null!;
    public VatNumber VatNumber { get; private set; } = null!;
    public EmailAddress Email { get; private set; } = null!;
    public PhoneNumber? Phone { get; private set; }
    public PostalAddress Address { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    public static Customer Create(
        CustomerName name,
        VatNumber vatNumber,
        EmailAddress email,
        PostalAddress address,
        PhoneNumber? phone = null)
        => new()
        {
            Id = CustomerId.New(),
            Name = Guard.NotNull(name, "Customer name"),
            VatNumber = Guard.NotNull(vatNumber, "VAT number"),
            Email = Guard.NotNull(email, "Email"),
            Address = Guard.NotNull(address, "Address"),
            Phone = phone,
            IsActive = true
        };

    public void Rename(CustomerName name) => Name = Guard.NotNull(name, "Customer name");

    public void ChangeContact(EmailAddress email, PhoneNumber? phone)
    {
        Email = Guard.NotNull(email, "Email");
        Phone = phone;
    }

    public void Relocate(PostalAddress address) => Address = Guard.NotNull(address, "Address");

    public void Deactivate() => IsActive = false;
    public void Reactivate() => IsActive = true;

    public BillTo ToBillTo() => new(Name, VatNumber, Address);
}
