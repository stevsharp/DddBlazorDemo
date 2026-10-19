using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Infrastructure.Outbox;

/// <summary>
/// Maps domain event CLR types to the stable names stored in the outbox, in both directions.
/// Every event that can be raised must be registered here or the outbox will refuse it.
/// </summary>
public sealed class DomainEventTypeRegistry
{
    private readonly Dictionary<string, Type> _byName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Type, string> _byType = new();

    public DomainEventTypeRegistry Register<TEvent>(string name) where TEvent : IDomainEvent
    {
        _byName[name] = typeof(TEvent);
        _byType[typeof(TEvent)] = name;
        return this;
    }

    public Type Resolve(string name)
        => _byName.TryGetValue(name, out var t)
            ? t
            : throw new InvalidOperationException($"Unknown event type '{name}'. Register it in DomainEventTypeRegistry.");

    public string NameOf(IDomainEvent evt)
        => _byType.TryGetValue(evt.GetType(), out var name)
            ? name
            : throw new InvalidOperationException($"No outbox name registered for event type '{evt.GetType().Name}'. Register it in DomainEventTypeRegistry.");
}
