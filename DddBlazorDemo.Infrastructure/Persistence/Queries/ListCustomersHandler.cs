using DddBlazorDemo.Application.Billing.Queries;
using DddBlazorDemo.Application.Repositories;
using DddBlazorDemo.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Billing.Domain;

namespace DddBlazorDemo.Infrastructure.Persistence.Queries;

public sealed class ListCustomersHandler(IReadOnlyRepository<Customer, CustomerId> customers)
    : IQueryHandler<ListCustomersQuery, IReadOnlyList<CustomerListItemDto>>
{
    public async Task<IReadOnlyList<CustomerListItemDto>> Handle(ListCustomersQuery query, CancellationToken ct)
    {
        // Filter and order run in SQL. The projection runs in memory: EF Core cannot translate
        // member access on value-converted types (c.Name.Value), only the property itself.
        var list = await customers.ListAsync(q => q
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name), ct);

        return [.. list.Select(c => new CustomerListItemDto(
            c.Id.Value,
            c.Name.Value,
            c.VatNumber.Value,
            c.Email.Value,
            c.Phone?.Value,
            c.Address.City,
            c.Address.CountryCode))];
    }
}
