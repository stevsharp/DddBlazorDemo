using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Application.DomainEvents;

/// <summary>
/// Reacts to a domain event delivered asynchronously by the outbox processor.
/// Runs inside a unit of work, so implementations must not call SaveChanges.
/// </summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent evt, CancellationToken ct);
}
