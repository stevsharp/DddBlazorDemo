using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Billing.Domain;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class CreateCustomerHandler(ICustomerRepository customers)
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    public Task<Guid> Handle(CreateCustomerCommand command, CancellationToken ct)
    {
        // Each value object validates itself; any failure is a DomainException the UI shows as-is.
        var customer = Customer.Create(
            new CustomerName(command.Name),
            new VatNumber(command.VatNumber),
            new EmailAddress(command.Email),
            new PostalAddress(command.Street, command.City, command.PostalCode, command.CountryCode),
            string.IsNullOrWhiteSpace(command.Phone) ? null : new PhoneNumber(command.Phone));

        customers.Add(customer);

        return Task.FromResult(customer.Id.Value);
    }
}
