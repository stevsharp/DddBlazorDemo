
namespace DddBlazorDemo.Application.Cqrs;

public sealed class Dispatcher(IServiceProvider sp, IUnitOfWork uow) : IDispatcher
{
    private readonly IServiceProvider _sp = sp;
    private readonly IUnitOfWork _uow = uow;

    public Task<TResult> Send<TCommand, TResult>(TCommand command, CancellationToken ct)
        where TCommand : ICommand<TResult>
    {
        return _uow.Execute(async token =>
        {
            var handler = _sp.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            return await handler.Handle(command, token);
        }, ct);
    }

    public Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResult>
    {
        var handler = _sp.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return handler.Handle(query, ct);
    }
}
