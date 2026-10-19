using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.DomainEvents;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Billing.Application.Handlers;
using DddBlazorDemo.Billing.Domain.Events;
using DddBlazorDemo.Documents.Application;
using Microsoft.Extensions.DependencyInjection;

namespace DddBlazorDemo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher, Dispatcher>();

        // Command handlers (registered explicitly; no assembly scanning)
        services.AddScoped<ICommandHandler<CreateInvoiceCommand, Guid>, CreateInvoiceHandler>();
        services.AddScoped<ICommandHandler<AddInvoiceLineCommand, Unit>, AddInvoiceLineHandler>();
        services.AddScoped<ICommandHandler<ValidateInvoiceCommand, Unit>, ValidateInvoiceHandler>();

        // Domain event handlers (invoked by the outbox processor)
        services.AddScoped<IDomainEventHandler<InvoiceValidated>, InvoiceValidatedHandler>();

        return services;
    }
}
