using DddBlazorDemo.Documents.Domain;

namespace DddBlazorDemo.Documents.Application;

public interface IDocumentRepository
{
    Task<bool> ExistsForInvoiceAsync(Guid invoiceId, CancellationToken ct);

    void Add(Document document);
}
