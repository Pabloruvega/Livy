# PaymentService

Microservicio de pagos para el TP de Livy. Solución .NET independiente (`PaymentService.sln`),
con la misma arquitectura Clean Architecture + CQRS/MediatR + EF Core que el e-commerce principal.

## Cómo correrlo

```bash
dotnet run --project src/PaymentService.Api
```

Levanta en `http://localhost:5090` (HTTPS en `https://localhost:7090`) — puerto distinto al
del e-commerce (`5204`/`7245`) para poder correr ambos servicios a la vez. Swagger en `/swagger`.

La base SQLite (`paymentservice.db`) ya tiene la migración `InitialCreate` aplicada. Si se borra
o se clona el repo de cero:

```bash
dotnet ef database update \
  --project src/PaymentService.Infrastructure \
  --startup-project src/PaymentService.Api
```

## Decisiones

- **Persistencia:** EF Core 8.0.11 + SQLite (mismos paquetes/versión que el e-commerce, que
  también usa SQLite — no SQL Server). Una tabla `PaymentTransactions`, un índice único por
  `OrderId` (evita duplicar el pago de una misma orden si el e-commerce reintenta la llamada)
  y otro por `TransactionCode`.
- **Regla de aprobación:** `amount < PaymentRules:ApprovalLimit` (configurable en `appsettings.json`,
  hoy `100000`). Vive en el dominio (`PaymentTransaction.Process`), no en el Handler.
- **Idempotencia:** `ProcessPaymentHandler` busca primero por `OrderId`; si ya existe una
  transacción para esa orden, devuelve la existente en vez de crear una nueva. Esto es lo que
  hace seguro reintentar la llamada desde el e-commerce (por timeout o por la política de Polly).
- **Auth entre servicios:** el endpoint no usa JWT (este servicio no tiene usuarios/roles propios,
  solo lo llama el e-commerce server-to-server). Se protege con una clave compartida por header
  (`X-Api-Key`, middleware propio) configurada en `Security:ApiKey` — más liviano que un JWT
  completo para un solo cliente conocido, pero evita dejar el endpoint totalmente abierto.
- **Contratos:** `PaymentResultDto` es lo único que cruza la frontera HTTP — nunca se expone
  `PaymentTransaction` (la entidad de dominio) ni el value object `Money`.

## Endpoints

| Método | Ruta                          | Descripción                                   |
|--------|-------------------------------|------------------------------------------------|
| POST   | `/api/payments/process`       | Procesa el pago de una orden (`OrderId`, `Amount`) |
| GET    | `/api/payments/order/{orderId}` | Consulta el resultado de pago de una orden   |

Ambos requieren el header `X-Api-Key` con el valor de `Security:ApiKey`.
