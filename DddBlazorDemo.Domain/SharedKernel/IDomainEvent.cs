namespace DddBlazorDemo.SharedKernel;

public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}
