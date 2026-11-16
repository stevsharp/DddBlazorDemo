using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Application.Repositories;

/// <summary>
/// Read side for query handlers. No tracking, no Add/Remove, never saves.
/// Command handlers must not use it: entities returned here are detached, so changes would be lost.
/// </summary>
public interface IReadOnlyRepository<T, TId>
    where T : Entity, IAggregateRoot<TId>
    where TId : notnull
{
    Task<T?> GetAsync(TId id, CancellationToken ct);

    Task<IReadOnlyList<T>> ListAsync(CancellationToken ct);

    /// <summary>Runs a Where/OrderBy/Select chain as a single SQL statement, projecting in the database.</summary>
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        Func<IQueryable<T>, IQueryable<TResult>> query, CancellationToken ct);
}
