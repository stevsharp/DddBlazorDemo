using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Billing.Application;
using DddBlazorDemo.Billing.Application.Queries;
using DddBlazorDemo.Billing.Domain.Events;
using DddBlazorDemo.Documents.Application;
using DddBlazorDemo.Documents.Application.Queries;
using DddBlazorDemo.Infrastructure.DomainEvents;
using DddBlazorDemo.Infrastructure.Mapping;
using DddBlazorDemo.Infrastructure.Outbox;
using DddBlazorDemo.Infrastructure.Persistence;
using DddBlazorDemo.Infrastructure.Persistence.Queries;
using DddBlazorDemo.Infrastructure.Persistence.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DddBlazorDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Persistence. AddDbContextFactory also registers AppDbContext as a scoped service,
        // which is what the unit of work, repositories and query handlers share.
        services.AddDbContextFactory<AppDbContext>(opt =>
            opt.UseSqlite(configuration.GetConnectionString("AppDb")));

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        // Read side
        var mapsterConfig = new TypeAdapterConfig();
        mapsterConfig.Scan(typeof(MapsterConfig).Assembly);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IQueryHandler<ListInvoicesQuery, IReadOnlyList<InvoiceListItemDto>>, ListInvoicesHandler>();
        services.AddScoped<IQueryHandler<ListDocumentsQuery, IReadOnlyList<DocumentListItemDto>>, ListDocumentsHandler>();

        // Outbox + domain event dispatch
        services.AddSingleton(new DomainEventTypeRegistry()
            .Register<InvoiceValidated>("Billing.InvoiceValidated"));

        services.AddSingleton<DomainEventSerializer>();
        services.AddScoped<OutboxWriter>();
        services.AddScoped<DomainEventInvoker>();

        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));
        services.AddHostedService<OutboxProcessor>();

        return services;
    }

    /// <summary>Creates the SQLite schema if it does not exist. The demo has no migrations.</summary>
    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync(ct);
    }
}
