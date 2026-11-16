using DddBlazorDemo.Cqrs;

namespace DddBlazorDemo.Billing.Application.Commands;

public sealed record CreateCustomerCommand(
    string Name,
    string VatNumber,
    string Email,
    string? Phone,
    string Street,
    string City,
    string PostalCode,
    string CountryCode) : ICommand<Guid>;
