using DddBlazorDemo.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Outbox;

public sealed class OutboxWriter
{
    private readonly DomainEventSerializer _serializer;

    public OutboxWriter(DomainEventSerializer serializer) => _serializer = serializer;

    public int CaptureDomainEventsToOutbox(DbContext db, Func<IDomainEvent, string> nameResolver)
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
                Type = nameResolver(evt),
                PayloadJson = _serializer.Serialize(evt),
                CreatedAtUtc = now
            });
        }

        return events.Count;
    }
}
