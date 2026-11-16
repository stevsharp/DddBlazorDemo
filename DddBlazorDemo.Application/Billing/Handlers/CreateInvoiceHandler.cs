using DddBlazorDemo.Cqrs;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Billing.Domain;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class CreateInvoiceHandler(
    IInvoiceRepository invoices, 
    ICustomerRepository customers) 
    : ICommandHandler<CreateInvoiceCommand, Guid>
{
    public async Task<Guid> Handle(CreateInvoiceCommand command, 
        CancellationToken ct)
    {
        var customer = await customers.GetAsync(new CustomerId(command.CustomerId), ct)
            ?? throw new DomainException($"Customer with id {command.CustomerId} not found.");

        Guard.That(customer.IsActive, "Cannot invoice an inactive customer.");

        var invoice = Invoice.Create(
            new InvoiceNumber(command.Number),
            new EanNumber(command.ProviderEan),
            new Currency(command.Currency),
            customer.Id,
            customer.ToBillTo()
            );

        invoices.Add(invoice);

        return invoice.Id;
    }
}
