
using DddBlazorDemo.Cqrs;
using DddBlazorDemo.Application.ReadModels;

namespace DddBlazorDemo.Application.Billing.Queries;

public sealed record ListCustomersQuery() : IQuery<IReadOnlyList<CustomerListItemDto>>;