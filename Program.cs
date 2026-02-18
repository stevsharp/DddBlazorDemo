using DddBlazorDemo.Application.Cqrs;
using DddBlazorDemo.Application.ReadModels;
using DddBlazorDemo.Billing.Application.Commands;
using DddBlazorDemo.Billing.Application.Handlers;
using DddBlazorDemo.Billing.Application.Queries;
using DddBlazorDemo.Billing.Domain.Events;
using DddBlazorDemo.Documents.Application;
using DddBlazorDemo.Documents.Application.Handlers;
using DddBlazorDemo.Documents.Application.Queries;
using DddBlazorDemo.Infrastructure;
using DddBlazorDemo.Infrastructure.DomainEvents;
using DddBlazorDemo.Infrastructure.Mapping;
using DddBlazorDemo.Infrastructure.Outbox;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("AppDb"));
});

// Mapster
var mapsterConfig = TypeAdapterConfig.GlobalSettings;
mapsterConfig.Scan(typeof(MapsterConfig).Assembly);
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// CQRS + UoW + Outbox
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<IDispatcher, Dispatcher>();

// Domain events invoke (used by outbox processor)
builder.Services.AddScoped<DomainEventInvoker>();

// Outbox event type registry
builder.Services.AddSingleton(new DomainEventTypeRegistry()
    .Register<InvoiceValidated>("Billing.InvoiceValidated"));

builder.Services.AddSingleton<DomainEventSerializer>();
builder.Services.AddScoped<OutboxWriter>();

builder.Services.Configure<OutboxOptions>(opt =>
{
    opt.PollSeconds = 2;
    opt.BatchSize = 20;
    opt.MaxAttempts = 10;
});

builder.Services.AddHostedService<OutboxProcessor>();

// Command handlers
builder.Services.AddScoped<ICommandHandler<CreateInvoiceCommand, Guid>, CreateInvoiceHandler>();
builder.Services.AddScoped<ICommandHandler<AddInvoiceLineCommand, Unit>, AddInvoiceLineHandler>();
builder.Services.AddScoped<ICommandHandler<ValidateInvoiceCommand, Unit>, ValidateInvoiceHandler>();

// Query handlers
builder.Services.AddScoped<IQueryHandler<ListInvoicesQuery, IReadOnlyList<InvoiceListItemDto>>, ListInvoicesHandler>();
builder.Services.AddScoped<IQueryHandler<ListDocumentsQuery, IReadOnlyList<DocumentListItemDto>>, ListDocumentsHandler>();

// Domain event handlers (invoked by outbox processor)
builder.Services.AddScoped<IDomainEventHandler<InvoiceValidated>, InvoiceValidatedHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<DddBlazorDemo.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
