# DddBlazorDemo

A small but complete **Domain-Driven Design** sample on **.NET 9 / Blazor Server**, showing how the pieces fit together end to end:

- Aggregates, value objects and strongly-typed ids
- CQRS with a hand-written dispatcher (no MediatR)
- Unit of work + **transactional outbox** for reliable domain events
- Two bounded contexts (`Billing`, `Documents`) integrated asynchronously
- Generic write / read-only repositories on EF Core
- A Blazor UI with reusable components (data grid, forms, toasts)

It is deliberately dependency-light so you can read every line: EF Core (SQLite), Mapster, and nothing else.

---

## Quick start

```powershell
git clone https://github.com/<you>/DddBlazorDemo.git
cd DddBlazorDemo
dotnet run --project DddBlazorDemo/DddBlazorDemo.csproj
```

Open <http://localhost:55400> (or <https://localhost:55399>). On Windows you can also double-click `run-demo.cmd`.

The SQLite database `DddBlazorDemo/app.db` is created on first start with `EnsureCreated()`. **There are no migrations**: after changing the EF mappings, delete `DddBlazorDemo/app.db*` and start again.

### Try the flow

1. **Customers** → add a customer. Every field is a value object that validates itself (VAT number, email, address…).
2. **Invoices** → create an invoice for that customer. The invoice stores a *Bill-To snapshot*; editing the customer later does not change it.
3. Add a few lines with **+ Line**, then **Validate** (two-step confirm; it's final).
4. **Documents** → press ↻ Refresh after a second or two. The outbox processor has picked up `InvoiceValidated` and the Documents context has created a document.

---

## Solution layout

```
Domain  ←  Application  ←  Infrastructure  ←  Web
```

| Project | Contains | References |
|---|---|---|
| `DddBlazorDemo.Domain` | `SharedKernel` (`Entity`, `IAggregateRoot<TId>`, `Guard`, `DomainException`), and per context the aggregates, value objects and events | nothing |
| `DddBlazorDemo.Application` | `Cqrs/` (`ICommand`, `IQuery`, `Dispatcher`, `IUnitOfWork`, `IRepository<T,TId>`, `IReadOnlyRepository<T,TId>`), commands, handlers, repository interfaces, read-model DTOs | Domain |
| `DddBlazorDemo.Infrastructure` | `AppDbContext`, `EfUnitOfWork`, EF repositories, query handlers, Mapster config, the outbox | Application, Domain |
| `DddBlazorDemo` | Blazor Server host, pages and shared components | Infrastructure, Application |

Bounded contexts appear as folders in every layer (`Billing/`, `Documents/`) and namespaces are context-first: `DddBlazorDemo.Billing.Domain`, `DddBlazorDemo.Billing.Application`.

---

## The domain

### Billing

```
Customer : IAggregateRoot<CustomerId>
├── Id         CustomerId        readonly record struct wrapping a Guid
├── Name       CustomerName      ≤ 200 chars
├── VatNumber  VatNumber         8–15 alphanumerics, normalised, unique
├── Email      EmailAddress      normalised lower-case
├── Phone      PhoneNumber?      optional, digits with optional '+'
├── Address    PostalAddress     street, city, postal code, ISO country
└── IsActive   bool

Invoice : IAggregateRoot<Guid>
├── Number, ProviderEan, Currency, Status
├── CustomerId  CustomerId       reference by id — no navigation property
├── BillTo      BillTo           snapshot: (CustomerName, VatNumber, PostalAddress)
└── Lines       InvoiceLine[]    child entities, OwnsMany
```

Rules the aggregates enforce: lines can only be added while Draft, an invoice needs lines and a positive total to validate, validation is one-way and raises `InvoiceValidated`.

**Why `Customer` is its own aggregate** — it has its own lifecycle and is shared by many invoices. Invoices reference it by id and keep a snapshot of what belongs on a printed invoice, so historical invoices stay correct when the customer moves.

### Value object conventions

- `sealed record` (or `readonly record struct` for ids), validated and normalised in the constructor via `Guard`
- Length limits are `public const` on the type and reused in the EF mapping (`HasMaxLength(CustomerName.MaxLength)`)
- Single-value objects map with a `ValueConverter`; multi-value ones (`PostalAddress`, `BillTo`) with `OwnsOne`

### Documents

`Document` is created from an `InvoiceValidated` event and walks a one-way workflow (`Created → PdfGenerated → ReadyToSend`). It is the only coupling between the two contexts and it is asynchronous.

---

## Request flow

```
Blazor page ──► IDispatcher.Send(command)
                    │  new DI scope per operation
                    ▼
              EfUnitOfWork.Execute
                    │  BEGIN TRANSACTION
                    ▼
              ICommandHandler ──► IRepository<T,TId>.GetAsync / Add
                    │              aggregate.DoSomething()  (may AddDomainEvent)
                    ▼
              SaveChanges
              OutboxWriter.CaptureDomainEventsToOutbox   ← events → OutboxMessage rows
              SaveChanges
              COMMIT
```

Queries bypass the unit of work: `IQueryHandler` implementations live in Infrastructure, read through `IReadOnlyRepository<T,TId>` (`AsNoTracking`) and return DTOs.

### Repositories

| | Write side | Read side |
|---|---|---|
| Interface | `IInvoiceRepository : IRepository<Invoice, Guid>` | `IReadOnlyRepository<T, TId>` used directly |
| Implementation | `EfRepository<T,TId>` base, one subclass per aggregate | `EfReadOnlyRepository<T,TId>`, one open generic |
| Methods | `GetAsync`, `Add` (+ `Remove` only where an aggregate opts in) | `GetAsync`, `ListAsync`, `ListAsync(query ⇒ …)` |
| Saves? | never — `IUnitOfWork` commits | never |

There is no `Update` (EF tracks loaded aggregates) and no `Save` on repositories (that would bypass the outbox).

---

## Transactional outbox

The invoice status change and the `OutboxMessage` row are written in **one transaction**, so an event can never be lost between "state changed" and "event published".

`OutboxProcessor` (a `BackgroundService`) then:

1. polls every `Outbox:PollSeconds` (default 2 s),
2. claims a row with a 30 s lease using `ExecuteUpdateAsync` (safe across multiple instances),
3. deserialises the event via `DomainEventTypeRegistry`,
4. runs every `IDomainEventHandler<TEvent>` inside a **new** unit of work (so handlers can raise further events),
5. marks the row processed, or stores `LastError` and retries up to `MaxAttempts`.

Handlers must be idempotent; `InvoiceValidatedHandler` checks `ExistsForInvoiceAsync` before creating a document.

The UI is pull-based: pages reload after each command, and the Documents page has a Refresh button because documents appear only after the next poll.

---

## Web project

```
Components/
├── Layout/      MainLayout, NavMenu (sidebar)
├── Pages/       Home (overview), Customers, Invoices, Documents
└── Shared/
    ├── CommandPage.cs     base class: IsBusy, Message, Run() with DomainException handling
    ├── StatusBanner       toast + busy bar
    ├── PageHeader         title, subtitle, action slot
    ├── Collapsible        fold-away card for forms
    ├── DataGrid<TItem>    generic table: sorting, empty state, row keys
    └── GridColumn<TItem>  column with Value / Format / template
```

Domain errors (`DomainException`) are shown to the user as-is; anything else is logged and shown as a generic message. The stylesheet is served through `MapStaticAssets` / `@Assets["app.css"]`, so it is fingerprinted and never stale.

---

## Adding things

Nothing is assembly-scanned; every registration is explicit.

| To add… | Do this |
|---|---|
| a command | command + handler in `Application/<Context>/`, register in `AddApplication()` |
| a query | query + DTO in Application, handler in `Infrastructure/Persistence/Queries`, register in `AddInfrastructure()` |
| a domain event | event in Domain, `DomainEventTypeRegistry.Register<T>("Context.Name")` in `AddInfrastructure()`, handler registered in `AddApplication()` |
| an aggregate | `IXRepository : IRepository<X, XId>` in Application, `XRepository : EfRepository<X, XId>` in Infrastructure, mapping in `AppDbContext`, then delete `app.db*` |

---

## Configuration

```json
"Outbox": {
  "PollSeconds": 2,
  "BatchSize": 20,
  "MaxAttempts": 10
}
```

---

## Roadmap / ideas

- EF Core migrations instead of `EnsureCreated`
- Strongly-typed `InvoiceId` / `DocumentId`
- Dapper or `SqlQuery<T>` for the read side
- Architecture tests (NetArchTest) enforcing the layer rules
- Live updates on the Documents page (timer or SignalR)
- MassTransit + a broker for cross-service events

---

## Connect with me

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Profile-blue)](https://www.linkedin.com/in/spyros-ponaris-913a6937/)
