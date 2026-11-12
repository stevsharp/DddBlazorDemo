using DddBlazorDemo.Billing.Application;
using DddBlazorDemo.Domain.Billing;

using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(AppDbContext db) : ICustomerRepository
{
    public Task<Customer?> GetAsync(Guid id, CancellationToken ct)
        => db.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);

    public void Add(Customer customer) => db.Customers.Add(customer);
}
