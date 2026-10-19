using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Billing.Domain;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class CreateInvoiceHandler(IInvoiceRepository invoices) : ICommandHandler<CreateInvoiceCommand, Guid>
{
    public Task<Guid> Handle(CreateInvoiceCommand command, CancellationToken ct)
    {
        var invoice = Invoice.Create(
            new InvoiceNumber(command.Number),
            new EanNumber(command.ProviderEan),
            new Currency(command.Currency));

        invoices.Add(invoice);

        return Task.FromResult(invoice.Id);
    }
}
