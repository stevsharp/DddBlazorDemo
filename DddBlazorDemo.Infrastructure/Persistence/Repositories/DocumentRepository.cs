using DddBlazorDemo.Documents.Application;
using DddBlazorDemo.Documents.Domain;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

public sealed class DocumentRepository(AppDbContext db)
    : EfRepository<Document, Guid>(db), IDocumentRepository
{
    public Task<bool> ExistsForInvoiceAsync(Guid invoiceId, CancellationToken ct)
        => Set.AnyAsync(d => d.InvoiceId == invoiceId, ct);
}
