using DddBlazorDemo.Application.Cqrs;

namespace DddBlazorDemo.Billing.Application.Commands;

public sealed record CreateCustomerCommand(string Name, string Gln) : ICommand<Guid>;
