using DddBlazorDemo.Cqrs;

namespace DddBlazorDemo.Billing.Application.Commands;

public sealed record AddInvoiceLineCommand(
    Guid InvoiceId,
    string Description,
    decimal Amount) : ICommand<Unit>;
