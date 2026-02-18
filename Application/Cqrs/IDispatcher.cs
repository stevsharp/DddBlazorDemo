namespace DddBlazorDemo.Application.Cqrs;

public interface IDispatcher
{
    Task<TResult> Send<TCommand, TResult>(TCommand command, CancellationToken ct)
        where TCommand : ICommand<TResult>;

    Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResult>;
}
