using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Infrastructure.DomainEvents;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent evt, CancellationToken ct);
}
