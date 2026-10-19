using DddBlazorDemo.Application.DomainEvents;
using DddBlazorDemo.Billing.Domain.Events;
using DddBlazorDemo.Documents.Domain;

namespace DddBlazorDemo.Documents.Application;

/// <summary>
/// Billing -> Documents integration: a validated invoice gets a document.
/// </summary>
public sealed class InvoiceValidatedHandler(IDocumentRepository documents) : IDomainEventHandler<InvoiceValidated>
{
    public async Task HandleAsync(InvoiceValidated evt, CancellationToken ct)
    {
        if (await documents.ExistsForInvoiceAsync(evt.InvoiceId, ct)) return;

        var doc = Document.CreateFromValidatedInvoice(evt.InvoiceId, evt.InvoiceNumber);
        doc.MarkPdfGenerated();
        doc.MarkReadyToSend();

        documents.Add(doc);
    }
}
