namespace DddBlazorDemo.Application.ReadModels;

public sealed record InvoiceListItemDto(
    Guid Id,
    string Number,
    string ProviderEan,
    string Status,
    decimal TotalAmount,
    string CurrencyCode);
