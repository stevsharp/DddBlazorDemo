using DddBlazorDemo.Cqrs;

namespace DddBlazorDemo.Billing.Application.Commands;

public sealed record CreateInvoiceCommand(
    string Number,
    string ProviderEan,
    string Currency,Guid CustomerId) : ICommand<Guid>;
