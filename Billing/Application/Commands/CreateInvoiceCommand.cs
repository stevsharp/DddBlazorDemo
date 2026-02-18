using DddBlazorDemo.Application.Cqrs;

namespace DddBlazorDemo.Billing.Application.Commands;

public sealed record CreateInvoiceCommand(
    string Number,
    string ProviderEan,
    string Currency) : ICommand<Guid>;
