using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Documents.Application.Queries;
using DddBlazorDemo.Infrastructure;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Documents.Application.Handlers;

public sealed class ListDocumentsHandler : IQueryHandler<ListDocumentsQuery, IReadOnlyList<DocumentListItemDto>>
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly IMapper _mapper;

    public ListDocumentsHandler(IDbContextFactory<AppDbContext> dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory;    
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DocumentListItemDto>> Handle(ListDocumentsQuery query, CancellationToken ct)
    {
        var docs = await _dbFactory.CreateDbContext().Documents
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);

        return [.. docs.Select(_mapper.Map<DocumentListItemDto>)];
    }
}
