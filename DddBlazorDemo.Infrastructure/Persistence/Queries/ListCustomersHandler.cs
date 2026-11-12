using DddBlazorDemo.Application.Billing.Queries;
using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;

using Mapster;

using MapsterMapper;

using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Queries;

public sealed class ListCustomersHandler(AppDbContext db, IMapper mapper)
    : IQueryHandler<ListCustomersQuery, IReadOnlyList<CustomerListItemDto>>
{
    public async Task<IReadOnlyList<CustomerListItemDto>> Handle(ListCustomersQuery query, CancellationToken ct)
    {
        var customers = await db.Customers
            .AsNoTracking()
            .ProjectToType<CustomerListItemDto>(mapper.Config)
            .ToListAsync(ct);

        return [..customers.Select(mapper.Map<CustomerListItemDto> )];
    }
}
