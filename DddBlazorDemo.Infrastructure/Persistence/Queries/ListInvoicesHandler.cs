using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Billing.Application.Queries;
using DddBlazorDemo.Billing.Domain;
using MapsterMapper;

namespace DddBlazorDemo.Infrastructure.Persistence.Queries;

public sealed class ListInvoicesHandler(IReadOnlyRepository<Invoice, Guid> invoices, IMapper mapper)
    : IQueryHandler<ListInvoicesQuery, IReadOnlyList<InvoiceListItemDto>>
{
    public async Task<IReadOnlyList<InvoiceListItemDto>> Handle(ListInvoicesQuery query, CancellationToken ct)
    {
        // Total() is a domain method and cannot be translated to SQL, so invoices are
        // loaded (with their lines) and mapped in memory.
        var list = await invoices.ListAsync(q => q.OrderByDescending(x => x.Id), ct);

        return [.. list.Select(mapper.Map<InvoiceListItemDto>)];
    }
}
