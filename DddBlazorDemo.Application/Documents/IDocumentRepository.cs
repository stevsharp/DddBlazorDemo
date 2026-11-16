using DddBlazorDemo.Application.Repositories;
using DddBlazorDemo.Cqrs;
using DddBlazorDemo.Documents.Domain;

namespace DddBlazorDemo.Documents.Application;

public interface IDocumentRepository : IRepository<Document, Guid>
{
    Task<bool> ExistsForInvoiceAsync(Guid invoiceId, CancellationToken ct);
}
