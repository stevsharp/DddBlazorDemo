# DddBlazorDemo

Blazor Server (Interactive Server Components) demo with:
- DDD aggregates + domain events
- CQRS (commands/queries) + UnitOfWork
- Outbox pattern (reliable domain event processing)
- Mapster mapping

Run:
- dotnet restore
- dotnet run

The app will create a SQLite `app.db` in the working directory on first run.
