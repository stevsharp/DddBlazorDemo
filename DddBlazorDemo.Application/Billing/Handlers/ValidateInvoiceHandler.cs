using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class ValidateInvoiceHandler(IInvoiceRepository invoices) : ICommandHandler<ValidateInvoiceCommand, Unit>
{
    public async Task<Unit> Handle(ValidateInvoiceCommand command, CancellationToken ct)
    {
        var invoice = await invoices.GetAsync(command.InvoiceId, ct)
                      ?? throw new DomainException("Invoice not found.");

        invoice.Validate();

        return Unit.Value;
    }
}
