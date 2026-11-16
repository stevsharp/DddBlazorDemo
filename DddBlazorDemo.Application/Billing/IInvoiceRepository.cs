using DddBlazorDemo.Application.Repositories;
using DddBlazorDemo.Cqrs;
using DddBlazorDemo.Billing.Domain;

namespace DddBlazorDemo.Billing.Application;

public interface IInvoiceRepository : IRepository<Invoice, Guid>
{
    /// <summary>Only Draft invoices may be removed; the command handler enforces this.</summary>
    void Remove(Invoice invoice);
}
