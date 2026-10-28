# Livy — Trabajo Final Integrador Backend 2026

Trabajo Final Integrador de **Backend (Optativa II)** — Tecnicatura Universitaria en Desarrollo de Software · UCCuyo · 2026.
**Opción 1: PaymentService.**

Un solo repositorio con dos soluciones .NET 8 independientes que se comunican por HTTP:

| Carpeta | Solución | Qué es | Puerto (perfil `http`) |
|---|---|---|---|
| [`Livy/`](Livy/README.md) | `Livy.sln` | E-commerce: usuarios, categorías, productos y órdenes | `5204` |
| [`PaymentService/`](PaymentService/README.md) | `PaymentService.sln` | Microservicio de pagos que consume Livy | `5090` |

```
repo/
├── README.md                 ← este archivo
├── Livy/
│   ├── Livy.sln
│   ├── README.md             ← detalle del e-commerce
│   └── src/  Livy.Domain · Livy.Application · Livy.Infrastructure · Livy.WebApi
└── PaymentService/
    ├── PaymentService.sln
    ├── README.md             ← detalle del microservicio
    └── src/  PaymentService.Domain · PaymentService.Application · PaymentService.Infrastructure · PaymentService.Api
```

---

## Arquitectura

Los dos servicios usan **Clean Architecture** con las mismas 4 capas:

| Capa | Livy | PaymentService | Contiene |
|---|---|---|---|
| Domain | `Livy.Domain` | `PaymentService.Domain` | Entidades, Value Objects, enums, excepciones de dominio, interfaces `IRepository`/`IUnitOfWork` |
| Application | `Livy.Application` | `PaymentService.Application` | Casos de uso (Commands/Queries de MediatR + Handlers), validadores FluentValidation, DTOs, contratos (`IProductRepository`, `IPaymentClient`, …) |
| Infrastructure | `Livy.Infrastructure` | `PaymentService.Infrastructure` | EF Core + SQLite, repositorios, migraciones; en Livy además JWT y el cliente HTTP hacia PaymentService |
| Presentation | `Livy.WebApi` | `PaymentService.Api` | Controllers, middlewares, `GlobalExceptionHandler`, `Program.cs` (composition root) |

**Regla de dependencia:** las referencias apuntan siempre hacia adentro.

```
WebApi / Api ──► Infrastructure ──► Application ──► Domain
     └──────────────────────────────────┘
```

- `Domain` no referencia a nadie.
- `Application` solo conoce a `Domain` y define *interfaces* (puertos) que implementa `Infrastructure`.
- La capa de presentación referencia a `Application` (para mandar Commands/Queries) y a `Infrastructure` solo para registrar las implementaciones en DI.

### Comunicación Livy → PaymentService

```
 Cliente (Swagger)                 Livy (:5204)                                    PaymentService (:5090)
 ───────────────                   ────────────                                    ──────────────────────
 POST /api/orders ──JWT──► OrdersController
                              │ _mediator.Send(CreateOrderCommand)
                              ▼
                          CreateOrderHandler ──► OrderPaymentProcessor
                                                     │ IPaymentClient (Application)
                                                     ▼
                                                 PaymentClient (Infrastructure)
                                                 HttpClient tipado vía IHttpClientFactory
                                                 + Polly (retry + timeout por intento)
                                                     │
                                                     │  POST /api/payments/process
                                                     │  Header  X-Api-Key: <Services:Payment:ApiKey>
                                                     │  Body    { "orderId": "...", "amount": 1234.00 }
                                                     └──────────────────────────────────────► ApiKeyMiddleware
                                                                                                   ▼
                                                                                            PaymentsController
                                                                                                   ▼
                                                                                            ProcessPaymentHandler
                                                                                                   ▼
                                                                                            PaymentTransaction.Process
                                                     ◄──────────────────────────────────────  { "status": "Approved" | "Rejected", ... }
```

`PaymentClient` está registrado con `services.AddHttpClient<IPaymentClient, PaymentClient>(...)`; la URL base
(`Services:Payment:BaseUrl`) y la clave (`Services:Payment:ApiKey`) salen de configuración.

---

## Requisitos

- **SDK de .NET 8** (probado con 8.0.427). También compila con el **SDK de .NET 10** (probado con 10.0.401):
  todos los proyectos tienen `TargetFramework` `net8.0`, así que para ejecutarlos alcanza con el runtime de .NET 8.
- **dotnet-ef** (herramienta de migraciones de EF Core):

```bash
# Instalar (primera vez) — misma versión que los paquetes de EF Core del proyecto
dotnet tool install --global dotnet-ef --version 8.0.11

# Si ya estaba instalada
dotnet tool update --global dotnet-ef --version 8.0.11

# Verificar
dotnet ef --version
```

No hace falta instalar ningún motor de base de datos: los dos servicios usan **SQLite** (archivos `.db` locales, no versionados).

---

## Cómo levantar

Todos los comandos se ejecutan **desde la raíz del repo**. Los bloques sirven igual en PowerShell y en bash; donde cambia la sintaxis hay una versión para cada uno.

### 1. Clonar

```bash
git clone https://github.com/Pabloruvega/Livy.git
cd Livy
```

### 2. Restaurar paquetes

```bash
dotnet restore Livy/Livy.sln
dotnet restore PaymentService/PaymentService.sln
```

### 3. Crear las bases de datos (migraciones)

PowerShell:

```powershell
dotnet ef database update --project PaymentService/src/PaymentService.Infrastructure --startup-project PaymentService/src/PaymentService.Api
dotnet ef database update --project Livy/src/Livy.Infrastructure --startup-project Livy/src/Livy.WebApi
```

bash:

```bash
dotnet ef database update \
  --project PaymentService/src/PaymentService.Infrastructure \
  --startup-project PaymentService/src/PaymentService.Api

dotnet ef database update \
  --project Livy/src/Livy.Infrastructure \
  --startup-project Livy/src/Livy.WebApi
```

Esto crea `PaymentService/src/PaymentService.Api/paymentservice.db` y `Livy/src/Livy.WebApi/ecommerce.db`.
La migración de Livy ya carga tres categorías: **Electrónica** (`a1b2c3d4-0000-0000-0000-000000000001`),
**Ropa** (`…0002`) y **Hogar** (`…0003`).

### 4. Ejecutar (dos terminales, primero PaymentService)

```bash
# Terminal 1
dotnet run --project PaymentService/src/PaymentService.Api
```

```bash
# Terminal 2
dotnet run --project Livy/src/Livy.WebApi
```

`dotnet run` usa el primer perfil de `launchSettings.json` (`http`), que fija `ASPNETCORE_ENVIRONMENT=Development`.
Ese entorno es necesario: las claves (JWT, ApiKey, usuario Admin) están en `appsettings.Development.json`
y en `appsettings.json` quedan vacías. Si faltan, el servicio **no arranca** y lo informa con un mensaje claro.

| Servicio | URL | Swagger |
|---|---|---|
| PaymentService | `http://localhost:5090` | http://localhost:5090/swagger |
| Livy | `http://localhost:5204` | http://localhost:5204/swagger |

> El perfil `https` usa `https://localhost:7090` (PaymentService) y `https://localhost:7245` (Livy).
> Livy apunta a PaymentService por `Services:Payment:BaseUrl` = `http://localhost:5090`.

---

## Usuario Admin de prueba

| Email | Contraseña |
|---|---|
| `admin@livy.com` | `Admin123!` |

(valores de `Admin:Email` / `Admin:Password` en `Livy/src/Livy.WebApi/appsettings.Development.json`)

**Cómo se crea:** el registro público (`POST /api/users/register`) siempre crea usuarios con rol `User`.
Al iniciar Livy, `DbSeeder.SeedAdminUserAsync` (en `Livy.WebApi`) revisa si existe algún Admin; si no hay ninguno,
registra ese usuario con `RegisterUserCommand` y lo promueve con `PromoteToAdminCommand`. Si ya hay un Admin, no hace nada.

A partir de ahí, solo un Admin puede crear otros Admins con **`PATCH /api/users/{id}/promote`**.

---

## Regla de negocio del PaymentService

- **Aprueba** el pago si `amount < PaymentRules:ApprovalLimit`; si no, lo **rechaza**.
  El límite está en `PaymentService/src/PaymentService.Api/appsettings.json` (`"ApprovalLimit": 100000`) y es configurable.
  Una orden de exactamente 100000 se rechaza.
- **Dónde vive:** en el dominio, en `PaymentTransaction.Process(orderId, amount, approvalLimit)`
  (`PaymentService.Domain/Entities/PaymentTransaction.cs`). El Handler solo lee el límite de configuración y persiste.
  El monto se modela con el Value Object `Money`, que rechaza montos `<= 0`.
- **Idempotencia por `OrderId`:** `ProcessPaymentHandler` busca primero una transacción para ese `OrderId`; si existe,
  devuelve la misma en vez de crear otra (además hay un índice único por `OrderId` en la tabla `PaymentTransactions`).
  Por eso Livy puede reintentar la llamada (Polly o `/pay`) sin cobrar dos veces.

---

## Estados de la orden (Livy)

`OrderStatus` (`Livy.Domain/Enums/OrderStatus.cs`). La orden se crea, se intenta cobrar y se guarda con su estado final en una sola transacción.

| Estado | Qué lo provoca |
|---|---|
| `Pending` | Estado inicial. Queda así si PaymentService no respondió (caído, timeout, reintentos agotados) o respondió con un error (por ejemplo 401 por ApiKey incorrecta). El stock queda **reservado**. Se puede reintentar con `/pay`. |
| `Paid` | PaymentService respondió `Approved` (total `< 100000`). Guarda el `paymentTransactionCode`. |
| `PaymentRejected` | PaymentService respondió `Rejected` (total `>= 100000`). Guarda el `paymentTransactionCode` y **devuelve el stock** de cada ítem. |
| `Confirmed`, `Shipped`, `Delivered`, `Cancelled` | Definidos en el enum, pero ningún caso de uso los asigna todavía. |

Solo una orden `Pending` puede pasar a `Paid` o `PaymentRejected`; intentar pagar una orden en otro estado devuelve **422**.

---

## Qué pasa si el PaymentService no responde

Configuración en `Livy/src/Livy.WebApi/appsettings.json` → `Services:Payment`:

| Clave | Valor | Efecto |
|---|---|---|
| `TimeoutPerAttemptSeconds` | `3` | Cada intento se corta a los 3 s (Polly `TimeoutAsync`, por intento) |
| `RetryCount` | `3` | Hasta 3 reintentos después del primer intento (4 intentos en total) |
| esperas entre reintentos | 1 s, 2 s, 4 s | Backoff exponencial (`2^(intento-1)` s, fijo en código) |
| `TotalTimeoutSeconds` | `25` | `HttpClient.Timeout` para toda la operación (peor caso calculado: 4 × 3 + 1 + 2 + 4 = 19 s) |

Se reintenta ante errores de red, respuestas 5xx o 408 y timeouts por intento. **No** se reintentan 400/401/404/422.

Si después de los reintentos no hay respuesta:

1. `PaymentClient` loguea el error y devuelve `null` (no lanza excepción).
2. La orden queda **`Pending`**, con el stock reservado, y se guarda igual.
3. El cliente recibe **201 Created** con la orden y
   `"paymentMessage": "No se pudo confirmar el pago en este momento. La orden quedó pendiente, podés reintentarlo más tarde."`
4. Cuando PaymentService vuelve a estar disponible, el dueño de la orden (o un Admin) reintenta con
   **`POST /api/orders/{id}/pay`** → 200 con la orden en `Paid` o `PaymentRejected`.

---

## Flujo end-to-end en Swagger

Con los dos servicios levantados, en http://localhost:5204/swagger:

1. **Login Admin** — `POST /api/users/login`
   ```json
   { "email": "admin@livy.com", "password": "Admin123!" }
   ```
   → 200 con `token`.
2. **Authorize** — botón 🔒 *Authorize*, pegar el `token` (sin el prefijo `Bearer`) → *Authorize*.
3. **Crear categoría** — `POST /api/categories` → 201
   ```json
   { "name": "Libros" }
   ```
   (o usar una de las sembradas, por ejemplo Electrónica `a1b2c3d4-0000-0000-0000-000000000001`).
4. **Crear producto** — `POST /api/products` → 201. Guardar el `id`.
   ```json
   { "name": "Notebook", "description": "Prueba TP", "price": 50000, "stock": 10, "categoryId": "a1b2c3d4-0000-0000-0000-000000000001" }
   ```
5. **Orden < 100000 → `Paid`** — `POST /api/orders` → 201, `"status": "Paid"`, `"paymentMessage": "Pago aprobado."`
   ```json
   { "items": [ { "productId": "<id del producto>", "quantity": 1 } ] }
   ```
6. **Orden ≥ 100000 → `PaymentRejected`** — mismo body con `"quantity": 2` (total 100000) → 201,
   `"status": "PaymentRejected"`, `"paymentMessage": "El pago fue rechazado por el servicio de pagos."`.
   `GET /api/products/{id}` muestra `stock: 9`: las 2 unidades se devolvieron.
7. **Apagar PaymentService** (Ctrl+C en su terminal) y crear otra orden con `"quantity": 1` → tarda unos segundos
   (reintentos) y responde 201 con `"status": "Pending"` y el mensaje de reintento. Guardar el `id` de la orden.
8. **Encender PaymentService** (`dotnet run --project PaymentService/src/PaymentService.Api`) y llamar
   `POST /api/orders/{id}/pay` → 200, `"status": "Paid"`.
9. **Usuario común → endpoint Admin (403)** — `POST /api/users/register`
   ```json
   { "email": "usera@livy.com", "name": "Usuario A", "password": "UserA123!" }
   ```
   Login con ese usuario, *Authorize* con su token y `POST /api/categories` → **403** (`"title": "Forbidden"`).
10. **Usuario B intentando ver la orden de A (403)** — con el token de A, crear una orden (`POST /api/orders`) y guardar su `id`.
    Registrar y loguear `userb@livy.com` / `UserB123!`, *Authorize* con su token y `GET /api/orders/{id de A}` →
    **403** con `"detail": "No tenés permiso para ver esta orden."`.

---

## Manejo de errores

Todos los errores salen en formato **ProblemDetails (RFC 7807)** con `Content-Type: application/problem+json`.
Lo resuelve un `GlobalExceptionHandler` (`IExceptionHandler` de .NET 8) registrado con
`AddExceptionHandler<GlobalExceptionHandler>().AddProblemDetails()` y `app.UseExceptionHandler()` como primer middleware.
Los controllers no tienen `try/catch`. Las 401/403/404 sin cuerpo (autenticación, rutas inexistentes) también salen
como ProblemDetails gracias a `app.UseStatusCodePages()`.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Resource not found",
  "status": 404,
  "detail": "Producto con Id '...' no encontrado.",
  "instance": "/api/products/...",
  "traceId": "00-...-...-00"
}
```

| Excepción / situación | Status | `title` | Ejemplos |
|---|---|---|---|
| `FluentValidation.ValidationException` | **400** | `Validation failed` (sin `detail`, con `errors` por propiedad) | nombre vacío, precio `<= 0`, orden sin ítems |
| JSON mal formado / tipos inválidos (`[ApiController]`) | **400** | `One or more validation errors occurred.` | body inválido |
| Sin token / token inválido, credenciales inválidas, falta `X-Api-Key` | **401** | `Unauthorized` | login fallido, PaymentService sin ApiKey |
| `ForbiddenAccessException` / rol insuficiente | **403** | `Forbidden` | ver la orden de otro usuario, usuario común en endpoint Admin |
| `NotFoundException` (`ProductNotFound`, `CategoryNotFound`, `OrderNotFound`, `UserNotFound`, `PaymentTransactionNotFound`) | **404** | `Resource not found` | Id inexistente |
| `DomainException` (incluye `InsufficientStockException`, `InvalidPaymentAmountException`) | **422** | `Business rule violated` | stock insuficiente, email o categoría duplicados, producto inactivo, pagar una orden que no está `Pending` |
| Cualquier otra | **500** | `An unexpected error occurred` (sin `detail` ni stacktrace) | — |

Los 4xx se loguean con `LogWarning` (`MÉTODO ruta → status: detalle`); los 500, con `LogError` y la excepción completa.

---

## Endpoints

### Livy (`http://localhost:5204`)

| Método | Ruta | Quién puede llamarlo |
|---|---|---|
| POST | `/api/users/register` | Público |
| POST | `/api/users/login` | Público |
| GET | `/api/users` | Admin |
| GET | `/api/users/{id}` | Autenticado: el propio usuario, o Admin para cualquiera |
| PATCH | `/api/users/{id}/promote` | Admin |
| GET | `/api/categories` | Público |
| GET | `/api/categories/{id}` | Público |
| POST | `/api/categories` | Admin |
| DELETE | `/api/categories/{id}` | Admin |
| GET | `/api/products` | Autenticado |
| GET | `/api/products/search?term=...` | Autenticado |
| GET | `/api/products/{id}` | Autenticado |
| POST | `/api/products` | Admin |
| PUT | `/api/products/{id}` | Admin |
| DELETE | `/api/products/{id}` | Admin |
| PATCH | `/api/products/{id}/activate` | Admin |
| PATCH | `/api/products/{id}/deactivate` | Admin |
| POST | `/api/orders` | Autenticado (la orden queda a nombre del usuario del token) |
| GET | `/api/orders/{id}` | Autenticado: el dueño de la orden, o Admin |
| POST | `/api/orders/{id}/pay` | Autenticado: el dueño de la orden, o Admin |

### PaymentService (`http://localhost:5090`)

| Método | Ruta | Quién puede llamarlo |
|---|---|---|
| POST | `/api/payments/process` | API key (`X-Api-Key`) |
| GET | `/api/payments/order/{orderId}` | API key (`X-Api-Key`) |

Swagger (`/swagger`) queda accesible sin ApiKey; para probar los endpoints desde ahí, usar *Authorize* con el valor de
`Security:ApiKey` de `PaymentService/src/PaymentService.Api/appsettings.Development.json`.

---

## Decisiones de diseño

- **CQRS con MediatR en todos los casos de uso.** Cada operación es un Command o una Query con su Handler, incluido el
  login (`LoginCommand` → `LoginHandler`, que devuelve `null` ante credenciales inválidas y el controller responde 401).
  Los controllers solo hacen `_mediator.Send(...)`. Un `ValidationBehavior` de MediatR corre los validadores de
  FluentValidation antes de cada Handler.
- **ApiKey entre servicios en lugar de JWT.** PaymentService no tiene usuarios ni roles: su único cliente es Livy,
  server-to-server. Una clave compartida en el header `X-Api-Key` (`ApiKeyMiddleware`) alcanza para no dejar el
  endpoint abierto, sin montar un emisor de tokens para un solo cliente conocido.
- **Value Objects:**
  - `Email` (Livy) — valida formato, normaliza (trim + minúsculas) e igualdad por valor; se persiste con un value converter.
  - `Money` (PaymentService) — `readonly record struct`, inmutable, rechaza montos `<= 0` con `InvalidPaymentAmountException`.
- **`GlobalExceptionHandler` en la capa Api y no en Infrastructure.** Depende de `HttpContext` y de `ProblemDetails`
  (ASP.NET Core), y la Unidad 1 ubica los filtros de excepción en Presentation. Infrastructure no tiene por qué conocer HTTP.
- **Fallo del servicio de pagos como resultado, no como excepción.** `IPaymentClient` devuelve `null` si PaymentService no
  responde: es un caso esperado en una integración entre servicios, que termina en una orden `Pending` reintentable.
- **Secretos de desarrollo versionados a propósito.** `appsettings.Development.json` se versiona para que el proyecto
  levante al clonarlo; `appsettings.json` deja esas claves vacías y la app falla al iniciar si faltan
  (`Jwt:Key` además debe tener al menos 32 caracteres).

---

## Tecnologías

Versiones tomadas de los `.csproj`:

| Paquete | Versión | Dónde | Uso |
|---|---|---|---|
| .NET / ASP.NET Core | `net8.0` | todos | Framework |
| Microsoft.EntityFrameworkCore (+ `.Sqlite`, `.Design`, `.Tools`) | 8.0.11 | Infrastructure (ambos) | ORM + SQLite + migraciones |
| MediatR | 14.1.0 | Application (ambos) | CQRS / Mediator |
| FluentValidation (+ `.DependencyInjectionExtensions`) | 12.1.1 | Application (ambos) | Validación de Commands |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.0 | Livy.Infrastructure | Autenticación JWT |
| System.IdentityModel.Tokens.Jwt | 7.5.0 | Livy.Infrastructure | Generación de tokens |
| BCrypt.Net-Next | 4.2.0 | Livy.Application | Hash de contraseñas |
| Microsoft.Extensions.Http.Polly | 10.0.11 | Livy.Infrastructure | Retry + timeout hacia PaymentService |
| System.Net.Http.Json | 10.0.11 | Livy.Infrastructure | `PostAsJsonAsync` / `ReadFromJsonAsync` |
| Swashbuckle.AspNetCore | 6.6.2 | WebApi / Api | Swagger |
| Microsoft.AspNetCore.OpenApi | 8.0.26 | WebApi / Api | OpenAPI |

> MediatR 14 es de Lucky Penny Software: en desarrollo loguea un aviso de licencia al iniciar, que no afecta el funcionamiento.
