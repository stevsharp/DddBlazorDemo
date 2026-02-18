using DddBlazorDemo.Billing.Domain.Events;
using DddBlazorDemo.Documents.Domain;
using DddBlazorDemo.Infrastructure;
using DddBlazorDemo.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Documents.Application;

public sealed class InvoiceValidatedHandler : IDomainEventHandler<InvoiceValidated>
{
    private readonly AppDbContext _db;

    public InvoiceValidatedHandler(AppDbContext db) => _db = db;

    public async Task HandleAsync(InvoiceValidated evt, CancellationToken ct)
    {
        var exists = await _db.Documents.AnyAsync(d => d.InvoiceId == evt.InvoiceId, ct);
        if (exists) return;

        var doc = Document.CreateFromValidatedInvoice(evt.InvoiceId, evt.InvoiceNumber);
        doc.MarkPdfGenerated();
        doc.MarkReadyToSend();

        _db.Documents.Add(doc);
    }
}
