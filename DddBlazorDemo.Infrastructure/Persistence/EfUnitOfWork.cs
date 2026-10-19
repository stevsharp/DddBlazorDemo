using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Infrastructure.Outbox;

namespace DddBlazorDemo.Infrastructure.Persistence;

/// <summary>
/// Wraps an operation in a transaction on the scoped <see cref="AppDbContext"/>, saves,
/// copies any domain events raised by tracked aggregates into the outbox, saves again and commits.
/// The handler, the repositories and this class all share the same context instance.
/// </summary>
public sealed class EfUnitOfWork(AppDbContext db, OutboxWriter outboxWriter) : IUnitOfWork
{
    public Task Execute(Func<CancellationToken, Task> operation, CancellationToken ct)
        => Execute(async token =>
        {
            await operation(token);
            return Unit.Value;
        }, ct);

    public async Task<TResult> Execute<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var result = await operation(ct);

        await db.SaveChangesAsync(ct);

        if (outboxWriter.CaptureDomainEventsToOutbox(db) > 0)
            await db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);

        return result;
    }
}
