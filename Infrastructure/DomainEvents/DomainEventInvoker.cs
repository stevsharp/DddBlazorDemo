using DddBlazorDemo.SharedKernel;

namespace DddBlazorDemo.Infrastructure.DomainEvents;

public sealed class DomainEventInvoker(IServiceProvider sp)
{
    private readonly IServiceProvider _sp = sp;

    public async Task InvokeAsync(IDomainEvent evt, CancellationToken ct)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(evt.GetType());
        var handlers = _sp.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("HandleAsync")!;

            var task = (Task)method.Invoke(handler, [evt, ct])!;

            await task;
        }
    }
}
