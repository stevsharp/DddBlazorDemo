using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;

namespace DddBlazorDemo.Documents.Application.Queries;

public sealed record ListDocumentsQuery() : IQuery<IReadOnlyList<DocumentListItemDto>>;
