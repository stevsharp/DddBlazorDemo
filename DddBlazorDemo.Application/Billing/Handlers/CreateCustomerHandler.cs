using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Billing.Application;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Domain.Billing;

namespace DddBlazorDemo.Application.Billing.Handlers;

public sealed class CreateCustomerHandler(ICustomerRepository customers)
    : ICommandHandler<CreateCustomerCommand, Guid>
{
    public Task<Guid> Handle(CreateCustomerCommand command, CancellationToken ct)
    {
       var customer = Customer.Create(command.Name, new DddBlazorDemo.Billing.Domain.EanNumber(command.Gln));

         customers.Add(customer);
    
         return Task.FromResult(customer.Id);
    }
}
