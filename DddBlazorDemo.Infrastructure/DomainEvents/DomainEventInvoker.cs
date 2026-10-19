using DddBlazorDemo.Application.DomainEvents;
using DddBlazorDemo.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace DddBlazorDemo.Infrastructure.DomainEvents;

/// <summary>
/// Resolves every <see cref="IDomainEventHandler{TEvent}"/> for the runtime type of an event
/// from the current scope and invokes them in registration order.
/// </summary>
public sealed class DomainEventInvoker(IServiceProvider sp)
{
    public async Task InvokeAsync(IDomainEvent evt, CancellationToken ct)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(evt.GetType());
        var method = handlerType.GetMethod("HandleAsync")!;

        foreach (var handler in sp.GetServices(handlerType))
        {
            await (Task)method.Invoke(handler, [evt, ct])!;
        }
    }
}
