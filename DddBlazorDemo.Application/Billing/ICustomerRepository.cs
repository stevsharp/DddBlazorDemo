using DddBlazorDemo.Domain.Billing;

namespace DddBlazorDemo.Billing.Application;

public interface ICustomerRepository
{
    Task<Customer?> GetAsync(Guid id, CancellationToken ct);

    void Add(Customer customer);
}