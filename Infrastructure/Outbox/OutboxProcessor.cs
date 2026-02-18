using DddBlazorDemo.Infrastructure.DomainEvents;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DddBlazorDemo.Infrastructure.Outbox;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly OutboxOptions _opt;
    private readonly string _owner = $"instance-{Guid.NewGuid():N}";

    public OutboxProcessor(IServiceProvider sp, IOptions<OutboxOptions> opt)
    {
        _sp = sp;
        _opt = opt.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOnce(stoppingToken);
            }
            catch
            {
            }

            await Task.Delay(TimeSpan.FromSeconds(_opt.PollSeconds), stoppingToken);
        }
    }

    private async Task ProcessOnce(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        var candidates = await db.OutboxMessages
            .AsNoTracking()
            .Where(x =>
                x.ProcessedAtUtc == null &&
                x.AttemptCount < _opt.MaxAttempts &&
                (x.LockedUntilUtc == null || x.LockedUntilUtc < now))
            .OrderBy(x => x.OccurredAtUtc)
            .Take(_opt.BatchSize)
            .Select(x => x.Id)
            .ToListAsync(ct);

        foreach (var id in candidates)
        {
            var claimed = await TryClaim(db, id, ct);
            if (!claimed) continue;

            await HandleOne(scope, id, ct);
        }
    }

    private async Task<bool> TryClaim(AppDbContext db, Guid id, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var leaseUntil = now.AddSeconds(30);

        var affected = await db.OutboxMessages
            .Where(x =>
                x.Id == id &&
                x.ProcessedAtUtc == null &&
                x.AttemptCount < _opt.MaxAttempts &&
                (x.LockedUntilUtc == null || x.LockedUntilUtc < now))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.LockOwner, _owner)
                .SetProperty(x => x.LockedUntilUtc, leaseUntil)
                .SetProperty(x => x.AttemptCount, x => x.AttemptCount + 1)
                .SetProperty(x => x.LastAttemptUtc, now)
                .SetProperty(x => x.LastError, (string?)null), ct);

        return affected == 1;
    }

    private static string Trunc(string s, int max) => s.Length <= max ? s : s[..max];

    private async Task HandleOne(IServiceScope scope, Guid id, CancellationToken ct)
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var invoker = scope.ServiceProvider.GetRequiredService<DomainEventInvoker>();
        var serializer = scope.ServiceProvider.GetRequiredService<DomainEventSerializer>();
        var registry = scope.ServiceProvider.GetRequiredService<DomainEventTypeRegistry>();
        var outboxWriter = scope.ServiceProvider.GetRequiredService<OutboxWriter>();

        var msg = await db.OutboxMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.LockOwner == _owner, ct);

        if (msg == null) return;

        try
        {
            var evt = serializer.Deserialize(msg.Type, msg.PayloadJson, registry);

            await invoker.InvokeAsync(evt, ct);

            await db.SaveChangesAsync(ct);

            outboxWriter.CaptureDomainEventsToOutbox(db, EventNameResolver.Resolve);
            await db.SaveChangesAsync(ct);

            await db.OutboxMessages
                .Where(x => x.Id == id && x.LockOwner == _owner)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ProcessedAtUtc, DateTime.UtcNow)
                    .SetProperty(x => x.LockOwner, (string?)null)
                    .SetProperty(x => x.LockedUntilUtc, (DateTime?)null), ct);
        }
        catch (Exception ex)
        {
            await db.OutboxMessages
                .Where(x => x.Id == id && x.LockOwner == _owner)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.LastError, Trunc(ex.Message, 2000))
                    .SetProperty(x => x.LockOwner, (string?)null)
                    .SetProperty(x => x.LockedUntilUtc, (DateTime?)null), ct);
        }
    }
}





