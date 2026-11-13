using DddBlazorDemo.Billing.Application;
using DddBlazorDemo.Billing.Domain;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(AppDbContext db)
    : EfRepository<Customer, CustomerId>(db), ICustomerRepository;
