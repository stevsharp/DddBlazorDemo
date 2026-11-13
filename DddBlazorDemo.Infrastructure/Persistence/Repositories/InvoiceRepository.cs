using DddBlazorDemo.Billing.Application;
using DddBlazorDemo.Billing.Domain;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository(AppDbContext db)
    : EfRepository<Invoice, Guid>(db), IInvoiceRepository
{
    public void Remove(Invoice invoice) => RemoveCore(invoice);
}
