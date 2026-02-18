using DddBlazorDemo.Billing.Domain.Events;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain;

public sealed class Invoice : Entity, IAggregateRoot
{
    private readonly List<InvoiceLine> _lines = new();
    private Invoice() { }
    public Guid Id { get; private set; }
    public InvoiceNumber Number { get; private set; } = new("N/A");
    public EanNumber ProviderEan { get; private set; } = new("00000000");
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;

    public Currency Currency { get; private set; } = Currency.EUR;
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<InvoiceLine> Lines => _lines;

    public static Invoice Create(InvoiceNumber number, EanNumber providerEan, Currency currency)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            Number = number,
            ProviderEan = providerEan,
            Currency = currency,
            Status = InvoiceStatus.Draft
        };
    }

    public void AddLine(string description, decimal amount)
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("You can only edit lines while the invoice is Draft.");

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
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Invoice is already validated.");

        if (_lines.Count == 0)
            throw new DomainException("Cannot validate an invoice without lines.");

        Status = InvoiceStatus.Validated;

        var total = Total();
        AddDomainEvent(new InvoiceValidated(
            InvoiceId: Id,
            InvoiceNumber: Number.Value,
            ProviderEan: ProviderEan.Value,
            TotalAmount: total.Amount,
            CurrencyCode: total.Currency.Code,
            OccurredAtUtc: DateTime.UtcNow));
    }
}
