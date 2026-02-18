using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Infrastructure.Outbox;

public sealed class DomainEventTypeRegistry
{
    private readonly Dictionary<string, Type> _map = new(StringComparer.OrdinalIgnoreCase);

    public DomainEventTypeRegistry Register<TEvent>(string name) where TEvent : IDomainEvent
    {
        _map[name] = typeof(TEvent);
        return this;
    }

    public Type Resolve(string name)
        => _map.TryGetValue(name, out var t)
            ? t
            : throw new InvalidOperationException($"Unknown event type '{name}'. Register it in DomainEventTypeRegistry.");
}
