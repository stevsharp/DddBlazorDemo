using Microsoft.Extensions.DependencyInjection;

namespace DddBlazorDemo.Application.Cqrs;

/// <summary>
/// Entry point for the UI. Each command or query runs in its own DI scope so that
/// scoped services (DbContext, unit of work, handlers) live only for that operation
/// rather than for the whole Blazor circuit.
/// </summary>
public sealed class Dispatcher(IServiceScopeFactory scopeFactory) : IDispatcher
{
    public async Task<TResult> Send<TCommand, TResult>(TCommand command, CancellationToken ct)
        where TCommand : ICommand<TResult>
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();

        return await uow.Execute(token => handler.Handle(command, token), ct);
    }

    public async Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResult>
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

        return await handler.Handle(query, ct);
    }
}
