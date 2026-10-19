using DddBlazorDemo.Documents.Application;
using DddBlazorDemo.Documents.Domain;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

public sealed class DocumentRepository(AppDbContext db) : IDocumentRepository
{
    public Task<bool> ExistsForInvoiceAsync(Guid invoiceId, CancellationToken ct)
        => db.Documents.AnyAsync(d => d.InvoiceId == invoiceId, ct);

    public void Add(Document document) => db.Documents.Add(document);
}
