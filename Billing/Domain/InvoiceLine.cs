using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed class InvoiceLine
{
    private InvoiceLine() { }

    public Guid Id { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = Money.Zero(Currency.EUR);

    public InvoiceLine(Guid id, string description, Money amount)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Line description is required.");
        if (amount.Amount <= 0) throw new DomainException("Line amount must be positive.");

        Id = id;
        Description = description.Trim();
        Amount = amount;
    }
}
