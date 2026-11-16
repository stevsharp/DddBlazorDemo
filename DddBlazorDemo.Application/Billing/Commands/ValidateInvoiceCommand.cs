using DddBlazorDemo.Cqrs;

namespace DddBlazorDemo.Billing.Application.Commands;

public sealed record ValidateInvoiceCommand(Guid InvoiceId) : ICommand<Unit>;
