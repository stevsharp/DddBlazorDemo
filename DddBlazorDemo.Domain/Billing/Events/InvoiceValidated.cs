using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Domain.Events;

public sealed record InvoiceValidated(
    Guid InvoiceId,
    string InvoiceNumber,
    string ProviderEan,
    decimal TotalAmount,
    string CurrencyCode,
    DateTime OccurredAtUtc) : IDomainEvent;
