using DddBlazorDemo.Billing.Application;
using DddBlazorDemo.Billing.Domain;

using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository(AppDbContext db) : IInvoiceRepository
{
    public Task<Invoice?> GetAsync(Guid id, CancellationToken ct)
        => db.Invoices.FirstOrDefaultAsync(x => x.Id == id, ct);

    public void Add(Invoice invoice) => db.Invoices.Add(invoice);
}
