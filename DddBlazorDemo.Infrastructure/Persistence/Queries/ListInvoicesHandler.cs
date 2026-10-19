using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Billing.Application.Queries;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Queries;

public sealed class ListInvoicesHandler(AppDbContext db, IMapper mapper)
    : IQueryHandler<ListInvoicesQuery, IReadOnlyList<InvoiceListItemDto>>
{
    public async Task<IReadOnlyList<InvoiceListItemDto>> Handle(ListInvoicesQuery query, CancellationToken ct)
    {
        var invoices = await db.Invoices
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync(ct);

        return [.. invoices.Select(mapper.Map<InvoiceListItemDto>)];
    }
}
