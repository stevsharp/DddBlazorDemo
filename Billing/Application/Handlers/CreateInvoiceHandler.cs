using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Billing.Domain;
using DddBlazorDemo.Infrastructure;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class CreateInvoiceHandler(AppDbContext db) : ICommandHandler<CreateInvoiceCommand, Guid>
{
    public Task<Guid> Handle(CreateInvoiceCommand command, CancellationToken ct)
    {
        var invoice = Invoice.Create(
            new InvoiceNumber(command.Number),
            new EanNumber(command.ProviderEan),
            new Currency(command.Currency));

        db.Invoices.Add(invoice);

        return Task.FromResult(invoice.Id);
    }
}
