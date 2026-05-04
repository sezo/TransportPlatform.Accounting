# TransportPlatform.Accounting — Claude context

## Bounded context
Financial accounting for ticket sales. Owns the company ledger (credits/debits per ticket),
customer registry, and employee records. Participates in the TicketPurchaseSaga as the
payment processor — responds to TicketReserved with PaymentProcessed or PaymentFailed.

## Team ownership
**Team 3** owns this repo.

## Business rules (domain layer enforces these)
- Ledger entries are created Pending, confirmed after payment, reversed on cancellation
- A customer must be at least 16 years old
- A customer email must be unique across the registry
- Employee salary cannot be negative
- Ledger balance = sum of confirmed credits − sum of confirmed debits

## Architecture
Clean Architecture (Onion):
- `TransportPlatform.Accounting.Domain` — zero dependencies, pure C#
- `TransportPlatform.Accounting.Application` — depends on Domain only, holds event consumers
- `TransportPlatform.Accounting.Infrastructure` — EF Core, MassTransit, repositories
- `TransportPlatform.Accounting.Api` — controllers, middleware, DI wiring

## Integration events consumed
- **TicketReserved** (Ticketing) → creates pending ledger entry, publishes PaymentProcessed
- **TicketCancelled** (Ticketing) → reverses ledger entry, publishes PaymentRefunded
- **TicketConfirmed** (Ticketing) → publishes InvoiceFiscalized (demo fiscal stub)

## Integration events published
- **PaymentProcessed** → Ticketing saga (advances to CapacityReserved wait)
- **PaymentFailed** → Ticketing saga (triggers compensation) — not currently triggered in demo
- **PaymentRefunded** → Reporting / external systems
- **InvoiceFiscalized** → Reporting / fiscal compliance systems

## Database
PostgreSQL 16 — dedicated instance, no sharing.
Connection string: `Host=postgres-accounting;Port=5432;Database=accounting;Username=transport;Password=transport`
External port (local dev): 5433
Migrations: `dotnet ef migrations add <Name> --project TransportPlatform.Accounting.Infrastructure --startup-project TransportPlatform.Accounting.Api`

## Running locally
```bash
# Ensure infra is running first
cd ../../_transport-platform-meta/infra && docker compose up -d

# Start accounting service
docker compose up -d

# Service available at http://localhost:5101
# Swagger UI at http://localhost:5101/swagger (Development only)
```

## API endpoints
- `GET  /api/ledger/summary` — current balance + credit/debit totals
- `GET  /api/ledger/entries` — paged ledger entries
- `GET  /api/customers` — paged customer list
- `GET  /api/customers/{id}` — customer by ID
- `POST /api/customers` — register new customer
- `GET  /api/employees` — paged employee list
- `GET  /api/employees/{id}` — employee by ID
- `POST /api/employees` — add new employee

All endpoints require `permission:accounting:write` (read endpoints use `permission:accounting:read`).

## What NOT to do
- Never query the ticketing or vehicles database directly
- Never implement payment gateway logic here — this is an accounting record, not a payment processor
- Never share Employee salary data in public-facing DTOs without access control
- Never put fiscal logic in the domain — the FiscalServiceStub is intentionally in Infrastructure
- Never publish events directly to RabbitMQ — always use IEventPublisher
