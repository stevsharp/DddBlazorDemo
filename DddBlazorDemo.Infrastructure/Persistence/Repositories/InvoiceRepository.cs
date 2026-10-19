using DddBlazorDemo.Billing.Application;
using DddBlazorDemo.Billing.Domain;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository(AppDbContext db) : IInvoiceRepository
{
    // Lines are an owned collection and are loaded together with the invoice.
    public Task<Invoice?> GetAsync(Guid id, CancellationToken ct)
        => db.Invoices.FirstOrDefaultAsync(x => x.Id == id, ct);

    public void Add(Invoice invoice) => db.Invoices.Add(invoice);
}
