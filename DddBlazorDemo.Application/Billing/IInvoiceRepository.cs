using DddBlazorDemo.Billing.Domain;

namespace DddBlazorDemo.Billing.Application;

public interface IInvoiceRepository
{
    Task<Invoice?> GetAsync(Guid id, CancellationToken ct);

    void Add(Invoice invoice);
}
