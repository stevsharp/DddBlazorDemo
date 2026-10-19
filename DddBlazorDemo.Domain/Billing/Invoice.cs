using DddBlazorDemo.Billing.Domain.Events;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed class Invoice : Entity, IAggregateRoot
{
    public const int MaxLines = 500;

    private readonly List<InvoiceLine> _lines = new();
    private Invoice() { }
    public Guid Id { get; private set; }
    public InvoiceNumber Number { get; private set; } = new("N/A");
    public EanNumber ProviderEan { get; private set; } = new("00000000");
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;

    public Currency Currency { get; private set; } = Currency.EUR;

    public IReadOnlyList<InvoiceLine> Lines => _lines;

    public static Invoice Create(InvoiceNumber number, EanNumber providerEan, Currency currency)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            Number = Guard.NotNull(number, "Invoice number"),
            ProviderEan = Guard.NotNull(providerEan, "Provider EAN"),
            Currency = Guard.NotNull(currency, "Currency"),
            Status = InvoiceStatus.Draft
        };
    }

    public void AddLine(string description, decimal amount)
    {
        EnsureDraft("You can only edit lines while the invoice is Draft.");
        Guard.That(_lines.Count < MaxLines, $"An invoice cannot have more than {MaxLines} lines.");

        var money = new Money(amount, Currency);

        _lines.Add(new InvoiceLine(Guid.NewGuid(), description, money));
    }

    public Money Total()
    {
        var total = Money.Zero(Currency);
        foreach (var l in _lines)
            total = total.Add(l.Amount);

        return total;
    }

    public void Validate()
    {
        EnsureDraft("Invoice is already validated.");
        Guard.That(_lines.Count > 0, "Cannot validate an invoice without lines.");

        var total = Total();
        Guard.That(total.Amount > 0, "Cannot validate an invoice with a zero total.");

        Status = InvoiceStatus.Validated;

        AddDomainEvent(new InvoiceValidated(
            InvoiceId: Id,
            InvoiceNumber: Number.Value,
            ProviderEan: ProviderEan.Value,
            TotalAmount: total.Amount,
            CurrencyCode: total.Currency.Code,
            OccurredAtUtc: DateTime.UtcNow));
    }

    private void EnsureDraft(string message) => Guard.That(Status == InvoiceStatus.Draft, message);
}
