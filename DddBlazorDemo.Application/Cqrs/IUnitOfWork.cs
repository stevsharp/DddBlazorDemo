namespace DddBlazorDemo.Application.Cqrs;

public interface IUnitOfWork
{
    Task<TResult> Execute<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken ct);
    Task Execute(Func<CancellationToken, Task> operation, CancellationToken ct);
}
