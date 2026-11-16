using DddBlazorDemo.Application.Repositories;
using DddBlazorDemo.Cqrs;
using DddBlazorDemo.Billing.Domain;

namespace DddBlazorDemo.Billing.Application;

// No Remove: customers are deactivated, never deleted.
public interface ICustomerRepository : IRepository<Customer, CustomerId> { }
