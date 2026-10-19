namespace DddBlazorDemo.SharedKernel;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent evt) => _domainEvents.Add(evt);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
