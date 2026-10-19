using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Documents.Application.Queries;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Queries;

public sealed class ListDocumentsHandler(AppDbContext db, IMapper mapper)
    : IQueryHandler<ListDocumentsQuery, IReadOnlyList<DocumentListItemDto>>
{
    public async Task<IReadOnlyList<DocumentListItemDto>> Handle(ListDocumentsQuery query, CancellationToken ct)
    {
        var docs = await db.Documents
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);

        return [.. docs.Select(mapper.Map<DocumentListItemDto>)];
    }
}
