# DddBlazorDemo

Blazor Server (Interactive Server Components) demo with:
- DDD aggregates + domain events
- CQRS (commands/queries) + UnitOfWork
- Outbox pattern (reliable domain event processing)
- Mapster mapping

## Solution layout

| Project | Role |
| --- | --- |
| `DddBlazorDemo.Domain` | Aggregates, value objects and domain events per bounded context (`Billing`, `Documents`) plus the `SharedKernel`. No dependencies. |
| `DddBlazorDemo.Application` | CQRS abstractions and `Dispatcher`, commands, command handlers, domain event handlers, repository interfaces and read-model DTOs. Depends on Domain only. |
| `DddBlazorDemo.Infrastructure` | EF Core (`AppDbContext`, unit of work, repositories, query handlers), Mapster config and the outbox. Depends on Application and Domain. |
| `DddBlazorDemo` | Blazor Server host and pages. Wires everything through `AddApplication()` and `AddInfrastructure()`. |

## Run

```
dotnet run --project DddBlazorDemo/DddBlazorDemo.csproj
```

The app creates a SQLite `app.db` next to the web project on first run (`EnsureCreated`, no migrations).
Outbox polling settings live under the `Outbox` section of `appsettings.json`.
