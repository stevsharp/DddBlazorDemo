using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure.Persistence.Repositories;

public sealed class EfReadOnlyRepository<T, TId>(AppDbContext db) : IReadOnlyRepository<T, TId>
    where T : Entity, IAggregateRoot<TId>
    where TId : notnull
{
    private IQueryable<T> Query => db.Set<T>().AsNoTracking();

    // x.Id.Equals(id) is translated by EF to a key comparison, through the converter if there is one.
    public Task<T?> GetAsync(TId id, CancellationToken ct)
        => Query.FirstOrDefaultAsync(x => x.Id.Equals(id), ct);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct)
        => await Query.ToListAsync(ct);

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        Func<IQueryable<T>, IQueryable<TResult>> query, CancellationToken ct)
        => await query(Query).ToListAsync(ct);
}
