using DddBlazorDemo.Billing.Domain.Events;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Infrastructure.Outbox;

public static class EventNameResolver
{
    public static string Resolve(IDomainEvent evt) => evt switch
    {
        InvoiceValidated => "Billing.InvoiceValidated",
        _ => throw new InvalidOperationException($"No outbox name mapping for event type '{evt.GetType().Name}'.")
    };
}
