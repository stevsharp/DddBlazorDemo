using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Infrastructure.DomainEvents;
using DddBlazorDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DddBlazorDemo.Infrastructure.Outbox;

/// <summary>
/// Polls the outbox, claims messages with a short lease and dispatches each one to its
/// domain event handlers inside its own DI scope and unit of work. Events raised by those
/// handlers are captured back into the outbox by the unit of work, so handlers can chain.
/// </summary>
public sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxOptions> options,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private static readonly TimeSpan Lease = TimeSpan.FromSeconds(30);

    private readonly OutboxOptions _opt = options.Value;
    private readonly string _owner = $"instance-{Guid.NewGuid():N}";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOnce(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(_opt.PollSeconds), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox polling iteration failed.");
            }
        }
    }

    private async Task ProcessOnce(CancellationToken ct)
    {
        List<Guid> candidates;

        using (var scope = scopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var now = DateTime.UtcNow;

            candidates = await db.OutboxMessages
                .AsNoTracking()
                .Where(x =>
                    x.ProcessedAtUtc == null &&
                    x.AttemptCount < _opt.MaxAttempts &&
                    (x.LockedUntilUtc == null || x.LockedUntilUtc < now))
                .OrderBy(x => x.OccurredAtUtc)
                .Take(_opt.BatchSize)
                .Select(x => x.Id)
                .ToListAsync(ct);
        }

        foreach (var id in candidates)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!await TryClaim(db, id, ct)) continue;

            await HandleOne(scope.ServiceProvider, db, id, ct);
        }
    }

    private async Task<bool> TryClaim(AppDbContext db, Guid id, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var leaseUntil = now.Add(Lease);

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

    private async Task HandleOne(IServiceProvider sp, AppDbContext db, Guid id, CancellationToken ct)
    {
        var msg = await db.OutboxMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.LockOwner == _owner, ct);

        if (msg is null) return;

        try
        {
            var serializer = sp.GetRequiredService<DomainEventSerializer>();
            var registry = sp.GetRequiredService<DomainEventTypeRegistry>();
            var invoker = sp.GetRequiredService<DomainEventInvoker>();
            var uow = sp.GetRequiredService<IUnitOfWork>();

            var evt = serializer.Deserialize(msg.Type, msg.PayloadJson, registry);

            await uow.Execute(token => invoker.InvokeAsync(evt, token), ct);

            await db.OutboxMessages
                .Where(x => x.Id == id && x.LockOwner == _owner)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.ProcessedAtUtc, DateTime.UtcNow)
                    .SetProperty(x => x.LockOwner, (string?)null)
                    .SetProperty(x => x.LockedUntilUtc, (DateTime?)null), ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Outbox message {MessageId} ({Type}) failed on attempt {Attempt}.",
                id, msg.Type, msg.AttemptCount);

            await db.OutboxMessages
                .Where(x => x.Id == id && x.LockOwner == _owner)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.LastError, Trunc(ex.Message, 2000))
                    .SetProperty(x => x.LockOwner, (string?)null)
                    .SetProperty(x => x.LockedUntilUtc, (DateTime?)null), ct);
        }
    }

    private static string Trunc(string s, int max) => s.Length <= max ? s : s[..max];
}
