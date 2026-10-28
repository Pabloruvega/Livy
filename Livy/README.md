# 🛒 Livy — E-Commerce API

API REST con **ASP.NET Core 8**, **Clean Architecture**, **CQRS con MediatR** y **autenticación JWT**.
Consume el microservicio [PaymentService](../PaymentService/README.md) para cobrar las órdenes.

Proyecto integrador de la materia **Backend (Optativa II)** — Tecnicatura Universitaria en Desarrollo de Software · UCCuyo · 2026.
**Profesor:** Tec. Nicolás Ortiz

> Este archivo es el detalle del e-commerce. Cómo levantar los dos servicios, el flujo completo en Swagger,
> la tabla de endpoints y la de errores están en el [README principal](../README.md).

---

## 🏗️ Arquitectura

```
Livy/src/
├── Livy.Domain/          # Entidades, Value Objects (Email), enums, excepciones de dominio, IRepository/IUnitOfWork
├── Livy.Application/     # Commands/Queries + Handlers (MediatR), validadores, DTOs, contratos
├── Livy.Infrastructure/  # EF Core + SQLite, repositorios, migraciones, JWT, cliente HTTP de PaymentService
└── Livy.WebApi/          # Controllers, middlewares, GlobalExceptionHandler, DbSeeder, Program.cs
```

Regla de dependencia: `WebApi → Infrastructure → Application → Domain` (WebApi también referencia a Application).

### Flujo de una request HTTP

```
HTTP Request
    ↓  (UseExceptionHandler → UseStatusCodePages → RequestLoggingMiddleware → Authentication → Authorization)
Controller
    ↓  _mediator.Send(command/query)
MediatR → ValidationBehavior (FluentValidation; si falla → 400)
    ↓
Handler (caso de uso)
    ↓  repositorios + IUnitOfWork   /   IPaymentClient → PaymentService
Base de datos (SQLite)
    ↓
DTO → Controller → HTTP Response
```

### Capa Application (CQRS)

```
Livy.Application/
├── Common/
│   ├── Behaviors/        # ValidationBehavior (pipeline de MediatR)
│   └── Exceptions/       # ForbiddenAccessException (→ 403)
├── Contracts/
│   ├── External/         # IPaymentClient, IJwtTokenService
│   └── Persistence/      # IProductRepository, IOrderRepository, ICategoryRepository, IUserRepository
├── DTOs/                 # Category, Order, Product, User
└── Features/
    ├── Categories/  Commands: CreateCategory, DeleteCategory · Queries: GetAllCategories, GetCategoryById
    ├── Orders/      Commands: CreateOrder, PayOrder · Queries: GetOrderById · Common: OrderPaymentProcessor
    ├── Products/    Commands: Create, Update, Delete, Activate, Deactivate · Queries: GetAll, GetById, Search
    └── Users/       Commands: RegisterUser, Login, PromoteToAdmin · Queries: GetAllUsers, GetUserById
```

---

## ⚙️ Configuración

| Archivo | Qué tiene |
|---|---|
| `src/Livy.WebApi/appsettings.json` | Connection string, `Services:Payment` (URL, timeouts, reintentos), `Jwt` (Issuer, Audience, `ExpirationHours: 1`), logging. Las claves sensibles quedan en `""`. |
| `src/Livy.WebApi/appsettings.Development.json` | Valores de desarrollo: `Jwt:Key`, `Services:Payment:ApiKey`, `Admin:Email`, `Admin:Password`. |

Si `Jwt:Key` falta o tiene menos de 32 caracteres, o falta `Services:Payment:ApiKey`, la app no arranca.
`Services:Payment:ApiKey` tiene que coincidir con `Security:ApiKey` del PaymentService.

---

## 🔐 Autenticación JWT

- `POST /api/users/register` y `POST /api/users/login` son públicos, igual que `GET /api/categories` y `GET /api/categories/{id}`.
  El resto requiere token.
- El registro siempre crea usuarios con rol `User`. El primer Admin lo crea `DbSeeder` al iniciar
  (`admin@livy.com` / `Admin123!`); después, un Admin promueve a otros con `PATCH /api/users/{id}/promote`.
- El token dura `Jwt:ExpirationHours` (1 hora).

**Registro** — `POST /api/users/register` → `201 Created`

```json
{ "email": "pablo@gmail.com", "name": "Pablo", "password": "123456" }
```

```json
{ "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", "email": "pablo@gmail.com", "name": "Pablo", "role": "User", "createdAt": "2026-..." }
```

**Login** — `POST /api/users/login` → `200 OK`

```json
{ "email": "pablo@gmail.com", "password": "123456" }
```

```json
{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...", "email": "pablo@gmail.com", "name": "Pablo", "role": "User", "expires": "2026-..." }
```

En Swagger: botón **Authorize 🔒**, pegar el valor de `token` (sin `Bearer`) y **Authorize**.

### Casos de error (ProblemDetails)

| Escenario | Status | `title` / `detail` |
|---|---|---|
| Datos inválidos (email sin formato, contraseña de menos de 6 caracteres) | `400` | `Validation failed`, con `errors` por campo |
| Contraseña incorrecta o email inexistente | `401` | `Unauthorized` / `Credenciales inválidas.` |
| Sin token en el header | `401` | `Unauthorized` |
| Email ya registrado | `422` | `Business rule violated` / `El email '...' ya está registrado.` |

---

## 💳 Integración con PaymentService

- `IPaymentClient` (Application) → `PaymentClient` (Infrastructure), registrado con `AddHttpClient<IPaymentClient, PaymentClient>`.
- Header `X-Api-Key` con `Services:Payment:ApiKey`; URL base `Services:Payment:BaseUrl` (`http://localhost:5090`).
- Polly: timeout de `TimeoutPerAttemptSeconds` (3 s) por intento, `RetryCount` (3) reintentos con esperas de 1, 2 y 4 s,
  y `HttpClient.Timeout` = `TotalTimeoutSeconds` (25 s).
- `OrderPaymentProcessor` aplica el resultado: `Approved` → `Paid`; `Rejected` → `PaymentRejected` + devolución de stock;
  sin respuesta → `Pending` (reintentable con `POST /api/orders/{id}/pay`).

El detalle de estados y del comportamiento ante caídas está en el [README principal](../README.md#estados-de-la-orden-livy).

---

## 🗄️ Base de datos

SQLite, sin instalar nada. Connection string en `src/Livy.WebApi/appsettings.json`:

```json
{ "ConnectionStrings": { "DefaultConnection": "Data Source=ecommerce.db" } }
```

El archivo `ecommerce.db` se crea en `src/Livy.WebApi/` y no se versiona. La migración `AddCategoriesTable` siembra
las categorías Electrónica, Ropa y Hogar con GUIDs fijos (`a1b2c3d4-0000-0000-0000-00000000000{1,2,3}`).

### Migraciones (desde la raíz del repo)

```bash
# Aplicar migraciones pendientes (crea la base si no existe)
dotnet ef database update \
  --project Livy/src/Livy.Infrastructure \
  --startup-project Livy/src/Livy.WebApi

# Crear una nueva migración
dotnet ef migrations add NombreMigracion \
  --project Livy/src/Livy.Infrastructure \
  --startup-project Livy/src/Livy.WebApi

# Revertir todas las migraciones
dotnet ef database update 0 \
  --project Livy/src/Livy.Infrastructure \
  --startup-project Livy/src/Livy.WebApi
```

---

## 🛠️ Tecnologías

| Paquete | Versión | Proyecto |
|---|---|---|
| ASP.NET Core | `net8.0` | todos |
| Microsoft.EntityFrameworkCore (+ `.Sqlite`, `.Design`, `.Tools`) | 8.0.11 | Livy.Infrastructure |
| MediatR | 14.1.0 | Livy.Application |
| FluentValidation (+ `.DependencyInjectionExtensions`) | 12.1.1 | Livy.Application |
| BCrypt.Net-Next | 4.2.0 | Livy.Application |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.0 | Livy.Infrastructure |
| System.IdentityModel.Tokens.Jwt | 7.5.0 | Livy.Infrastructure |
| Microsoft.Extensions.Http.Polly | 10.0.11 | Livy.Infrastructure |
| System.Net.Http.Json | 10.0.11 | Livy.Infrastructure |
| Swashbuckle.AspNetCore | 6.6.2 | Livy.WebApi |
| Microsoft.AspNetCore.OpenApi | 8.0.26 | Livy.WebApi |
