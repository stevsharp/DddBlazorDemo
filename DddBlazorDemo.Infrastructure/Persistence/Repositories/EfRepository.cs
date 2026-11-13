using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Shared write-side code. Aggregates are tracked, so handler mutations are saved by EfUnitOfWork.
/// Owned types (invoice lines, BillTo, Address) load together with the aggregate.
/// </summary>
public abstract class EfRepository<T, TId>(AppDbContext db) : IRepository<T, TId>
    where T : Entity, IAggregateRoot<TId>
    where TId : notnull
{
    protected DbSet<T> Set => db.Set<T>();

    // FindAsync applies the key's ValueConverter, so a strongly-typed id works here unchanged.
    public async Task<T?> GetAsync(TId id, CancellationToken ct)
        => await Set.FindAsync([id], ct);

    public void Add(T aggregate) => Set.Add(aggregate);

    // Protected so deletion is opt-in per aggregate, not part of IRepository.
    protected void RemoveCore(T aggregate) => Set.Remove(aggregate);
}
