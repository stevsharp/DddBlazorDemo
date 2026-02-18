using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Billing.Application.Queries;
using DddBlazorDemo.Infrastructure;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Billing.Application.Handlers;

public sealed class ListInvoicesHandler(IDbContextFactory<AppDbContext> dbFactory, IMapper mapper) : IQueryHandler<ListInvoicesQuery, IReadOnlyList<InvoiceListItemDto>>
{
    private readonly IMapper _mapper = mapper;

    private readonly IDbContextFactory<AppDbContext> _dbFactory = dbFactory;

    public async Task<IReadOnlyList<InvoiceListItemDto>> Handle(ListInvoicesQuery query, CancellationToken ct)
    {
        var invoices = await _dbFactory.RunWithDbContext(c => c.Invoices
            .AsNoTracking()
            .Include("_lines")
            .OrderByDescending(x => x.Id)
            .ToListAsync(ct),ct);  

        return invoices.Select(_mapper.Map<InvoiceListItemDto>).ToList();
    }
}
