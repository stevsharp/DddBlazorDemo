using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class AddInvoiceLineHandler(IInvoiceRepository invoices) : ICommandHandler<AddInvoiceLineCommand, Unit>
{
    public async Task<Unit> Handle(AddInvoiceLineCommand command, CancellationToken ct)
    {
        var invoice = await invoices.GetAsync(command.InvoiceId, ct)
                      ?? throw new DomainException("Invoice not found.");

        invoice.AddLine(command.Description, command.Amount);

        return Unit.Value;
    }
}
