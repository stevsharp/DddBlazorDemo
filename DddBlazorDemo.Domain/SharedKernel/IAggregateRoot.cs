namespace DddBlazorDemo.SharedKernel;

/// <summary>Marker: only aggregate roots get repositories.</summary>
public interface IAggregateRoot { }

/// <summary>Aggregate root with a known identity type (a Guid or a strongly-typed id).</summary>
public interface IAggregateRoot<TId> : IAggregateRoot where TId : notnull
{
    TId Id { get; }
}
