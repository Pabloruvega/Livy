# PaymentService

Microservicio de pagos del TP de Livy (opción 1). Solución .NET 8 independiente (`PaymentService.sln`), con la misma
arquitectura que el e-commerce: Clean Architecture + CQRS/MediatR + EF Core + SQLite.

> Este archivo es el detalle del microservicio. Cómo levantar los dos servicios y el flujo completo están en el
> [README principal](../README.md).

## Estructura

```
PaymentService/src/
├── PaymentService.Domain/          # PaymentTransaction, Money (Value Object), PaymentStatus, excepciones de dominio
├── PaymentService.Application/     # ProcessPayment (Command), GetPaymentByOrderId (Query), validador, PaymentResultDto
├── PaymentService.Infrastructure/  # EF Core + SQLite, repositorio, migración InitialCreate
└── PaymentService.Api/             # PaymentsController, ApiKeyMiddleware, GlobalExceptionHandler, Program.cs
```

## Cómo correrlo

Desde la raíz del repo:

```bash
# Crear la base (la primera vez, o si se borró paymentservice.db)
dotnet ef database update \
  --project PaymentService/src/PaymentService.Infrastructure \
  --startup-project PaymentService/src/PaymentService.Api

dotnet run --project PaymentService/src/PaymentService.Api
```

Levanta en `http://localhost:5090` (perfil `https`: `https://localhost:7090`), con `ASPNETCORE_ENVIRONMENT=Development`.
El puerto es distinto al del e-commerce (`5204` / `7245`) para poder correr los dos a la vez. Swagger en
http://localhost:5090/swagger.

La base `paymentservice.db` se crea en `src/PaymentService.Api/` y no se versiona.

## Configuración

| Archivo | Qué tiene |
|---|---|
| `src/PaymentService.Api/appsettings.json` | Connection string `PaymentDb`, `PaymentRules:ApprovalLimit` (`100000`), logging. `Security:ApiKey` queda en `""`. |
| `src/PaymentService.Api/appsettings.Development.json` | `Security:ApiKey` de desarrollo (la misma que `Services:Payment:ApiKey` de Livy). |

Si `Security:ApiKey` está vacía, el servicio no arranca.

## Decisiones

- **Persistencia:** EF Core 8.0.11 + SQLite (mismos paquetes y versión que el e-commerce). Una tabla `PaymentTransactions`,
  con un índice único por `OrderId` (no se duplica el pago de una orden si el e-commerce reintenta) y otro por `TransactionCode`.
- **Regla de aprobación:** `amount < PaymentRules:ApprovalLimit` → `Approved`; si no → `Rejected` (100000 exacto se rechaza).
  Vive en el dominio (`PaymentTransaction.Process`), no en el Handler.
- **Value Object `Money`:** `readonly record struct`, inmutable; rechaza montos `<= 0` con `InvalidPaymentAmountException`.
- **Idempotencia:** `ProcessPaymentHandler` busca primero por `OrderId`; si ya existe una transacción para esa orden,
  devuelve la existente. Por eso es seguro que el e-commerce reintente (Polly o `POST /api/orders/{id}/pay`).
- **Auth entre servicios:** no usa JWT (no tiene usuarios ni roles propios; solo lo llama el e-commerce, server-to-server).
  Se protege con una clave compartida en el header `X-Api-Key` (`ApiKeyMiddleware`), configurada en `Security:ApiKey`.
  `/swagger` queda accesible sin clave.
- **Contratos:** `PaymentResultDto` es lo único que cruza la frontera HTTP; nunca se expone la entidad `PaymentTransaction` ni `Money`.

## Endpoints

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| POST | `/api/payments/process` | Procesa el pago de una orden. Body: `{ "orderId": "<guid>", "amount": 1234.00 }` | 200, 400, 401 |
| GET | `/api/payments/order/{orderId}` | Consulta el resultado del pago de una orden | 200, 401, 404 |

Los dos requieren el header `X-Api-Key` con el valor de `Security:ApiKey`.

Respuesta 200:

```json
{ "transactionId": "...", "orderId": "...", "status": "Approved", "transactionCode": "TX-1A2B3C4D", "amount": 1234.00, "processedAt": "2026-..." }
```

## Errores (ProblemDetails, `application/problem+json`)

| Situación | Status | `title` |
|---|---|---|
| Validación (`amount <= 0`, `orderId` vacío) | 400 | `Validation failed` (con `errors`) |
| Falta `X-Api-Key` o es incorrecta | 401 | `Unauthorized` (`detail`: `Header 'X-Api-Key' ausente o inválido.`) |
| No existe transacción para la orden (`PaymentTransactionNotFoundException`) | 404 | `Resource not found` |
| `DomainException` (por ejemplo `InvalidPaymentAmountException` si se saltea el validador) | 422 | `Business rule violated` |
| Cualquier otra | 500 | `An unexpected error occurred` |

## Tecnologías

| Paquete | Versión | Proyecto |
|---|---|---|
| ASP.NET Core | `net8.0` | todos |
| Microsoft.EntityFrameworkCore (+ `.Sqlite`, `.Design`, `.Tools`) | 8.0.11 | PaymentService.Infrastructure |
| MediatR | 14.1.0 | PaymentService.Application |
| FluentValidation (+ `.DependencyInjectionExtensions`) | 12.1.1 | PaymentService.Application |
| Microsoft.Extensions.Configuration.Abstractions / .Binder | 10.0.11 | PaymentService.Application |
| Swashbuckle.AspNetCore | 6.6.2 | PaymentService.Api |
| Microsoft.AspNetCore.OpenApi | 8.0.26 | PaymentService.Api |
