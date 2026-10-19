using DddBlazorDemo.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Outbox;

public sealed class OutboxWriter(DomainEventSerializer serializer, DomainEventTypeRegistry registry)
{
    /// <summary>
    /// Moves pending domain events from tracked aggregates into <see cref="OutboxMessage"/> rows
    /// on the same context. Returns the number of events captured; the caller saves.
    /// </summary>
    public int CaptureDomainEventsToOutbox(DbContext db)
    {
        var entities = db.ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        if (entities.Count == 0) return 0;

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        foreach (var e in entities) e.ClearDomainEvents();

        var outbox = db.Set<OutboxMessage>();
        var now = DateTime.UtcNow;

        foreach (var evt in events)
        {
            outbox.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredAtUtc = evt.OccurredAtUtc,
                Type = registry.NameOf(evt),
                PayloadJson = serializer.Serialize(evt),
                CreatedAtUtc = now
            });
        }

        return events.Count;
    }
}
