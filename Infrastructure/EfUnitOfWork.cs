using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Infrastructure.Outbox;

using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure;

public sealed class EfUnitOfWork(IDbContextFactory<AppDbContext> dbFactory, OutboxWriter outboxWriter) : IUnitOfWork
{
    /// <summary>
    /// 
    /// </summary>
    private readonly OutboxWriter _outboxWriter = outboxWriter;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task Execute(Func<CancellationToken, Task> operation, CancellationToken ct)
        => Execute(async token => { 
            await operation(token); return 0; }, ct);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="operation"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<TResult> Execute<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken ct)
    {
        await using var tx = await dbFactory.CreateDbContext().Database.BeginTransactionAsync(ct);

        var result = await operation(ct);

        await SaveChangesWithConcurrencyRetry(ct);

        _outboxWriter.CaptureDomainEventsToOutbox(dbFactory.CreateDbContext(), EventNameResolver.Resolve);

        await SaveChangesWithConcurrencyRetry(ct);

        await tx.CommitAsync(ct);

        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task SaveChangesWithConcurrencyRetry(CancellationToken ct)
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                await dbFactory.CreateDbContext().SaveChangesAsync(ct);
                return;
            }
            catch (DbUpdateConcurrencyException ex) when (attempt < 2)
            {
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync(ct);
                }
            }
        }
    }
}

