using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;

namespace DddBlazorDemo.Billing.Application.Queries;

public sealed record ListInvoicesQuery() : IQuery<IReadOnlyList<InvoiceListItemDto>>;
