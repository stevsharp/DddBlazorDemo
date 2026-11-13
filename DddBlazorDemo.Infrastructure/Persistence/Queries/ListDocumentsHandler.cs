using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Documents.Application.Queries;
using DddBlazorDemo.Documents.Domain;
using MapsterMapper;

namespace DddBlazorDemo.Infrastructure.Persistence.Queries;

public sealed class ListDocumentsHandler(IReadOnlyRepository<Document, Guid> documents, IMapper mapper)
    : IQueryHandler<ListDocumentsQuery, IReadOnlyList<DocumentListItemDto>>
{
    public async Task<IReadOnlyList<DocumentListItemDto>> Handle(ListDocumentsQuery query, CancellationToken ct)
    {
        var docs = await documents.ListAsync(q => q.OrderByDescending(x => x.CreatedAtUtc), ct);

        return [.. docs.Select(mapper.Map<DocumentListItemDto>)];
    }
}
