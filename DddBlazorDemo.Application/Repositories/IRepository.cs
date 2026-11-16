using DddBlazorDemo.Cqrs;
using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Application.Repositories;

/// <summary>
/// Write side. Returns tracked aggregates for command handlers to mutate.
/// There is no Save: <see cref="IUnitOfWork"/> commits once per command together with the outbox.
/// Delete is deliberately not here; an aggregate that may be deleted declares Remove on its own interface.
/// </summary>
public interface IRepository<T, TId>
    where T : Entity, IAggregateRoot<TId>
    where TId : notnull
{
    Task<T?> GetAsync(TId id, CancellationToken ct);

    void Add(T aggregate);
}
