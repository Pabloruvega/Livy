# 🛒 Livy — E-Commerce API

API REST desarrollada con **ASP.NET Core 8** aplicando **Clean Architecture**, **CQRS con MediatR** y **autenticación JWT**.

Proyecto integrador de la materia **Backend (Optativa II)** — Tecnicatura Universitaria en Desarrollo de Software · UCCuyo · 2026.  
**Profesor:** Tec. Nicolás Ortiz

---

## 🏗️ Arquitectura

```
Livy/
├── Livy.Domain/          # Entidades, Value Objects, Excepciones, Interfaces (núcleo)
├── Livy.Application/     # Casos de uso (Features), DTOs, Contratos, Services
├── Livy.Infrastructure/  # EF Core, Repositorios, Migraciones, JWT
└── Livy.WebApi/          # Controllers, Middleware, Program.cs
```

### Flujo de una request HTTP

```
HTTP Request
    ↓
Controller
    ↓  _mediator.Send(command/query)
MediatR
    ↓  enruta automáticamente
Handler (UseCase)
    ↓  IRepository + IUnitOfWork
Base de Datos (SQLite)
    ↓
Response DTO → Controller → HTTP Response
```

### Estructura de la capa Application (CQRS)

```
Livy.Application/
├── Contracts/
│   └── Persistence/          # Interfaces de repositorios (contratos)
├── Features/
│   ├── Products/
│   │   ├── Commands/          # CreateProduct, UpdateProduct, DeleteProduct
│   │   └── Queries/           # GetAllProducts, GetProductById
│   └── Orders/
│       ├── Commands/          # CreateOrder
│       └── Queries/           # GetOrderById
├── Services/                  # UserService (login/register), CategoryService
└── DTOs/                      # Objetos de transferencia de datos
```

---

## ✅ Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- [DB Browser for SQLite](https://sqlitebrowser.org/) *(opcional, para inspeccionar la BD)*

---

## 🚀 Cómo levantar el proyecto

### 1. Clonar el repositorio

```bash
git clone https://github.com/Pabloruvega/Livy.git
cd Livy
```

### 2. Restaurar paquetes NuGet

```bash
dotnet restore Livy/Livy.sln
```

### 3. Instalar la herramienta de migraciones *(solo la primera vez)*

```bash
dotnet tool install --global dotnet-ef
```

### 4. Aplicar las migraciones y crear la base de datos

```bash
dotnet ef database update \
  --project Livy/src/Livy.Infrastructure \
  --startup-project Livy/src/Livy.WebApi
```

> Esto crea automáticamente el archivo `ecommerce.db` en la carpeta `src/Livy.WebApi/`.

### 5. Ejecutar el proyecto

```bash
dotnet run --project Livy/src/Livy.WebApi
```

### 6. Abrir Swagger

```
http://localhost:5204/swagger
```

---

## 🔐 Autenticación JWT

Todos los endpoints (excepto `/register` y `/login`) requieren un token JWT.

### Paso 1 — Registrar un usuario

**`POST /api/users/register`**

```json
{
  "email": "pablo@gmail.com",
  "name": "Pablo",
  "password": "123456"
}
```

Respuesta `201 Created`:
```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "email": "pablo@gmail.com",
  "name": "Pablo",
  "role": "User",
  "createdAt": "2026-05-22T..."
}
```

### Paso 2 — Hacer login y obtener el token

**`POST /api/users/login`**

```json
{
  "email": "pablo@gmail.com",
  "password": "123456"
}
```

Respuesta `200 OK`:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "pablo@gmail.com",
  "name": "Pablo",
  "role": "User",
  "expires": "2026-05-22T..."
}
```

### Paso 3 — Usar el token en Swagger

1. Copiá el valor del campo `token`
2. Hacé clic en el botón **Authorize 🔒** (arriba a la derecha en Swagger)
3. Pegá el token y hacé clic en **Authorize**
4. Todos los endpoints quedan habilitados

### Casos de error

| Escenario | Status | Mensaje |
|---|---|---|
| Contraseña incorrecta | `401 Unauthorized` | `"Credenciales inválidas."` |
| Email ya registrado | `400 Bad Request` | `"El email '...' ya está registrado."` |
| Sin token en el header | `401 Unauthorized` | — |

---

## 📦 Endpoints disponibles

### Usuarios
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `POST` | `/api/users/register` | ❌ | Registrar nuevo usuario |
| `POST` | `/api/users/login` | ❌ | Login — devuelve token JWT |
| `GET` | `/api/users` | ✅ | Listar todos los usuarios |
| `GET` | `/api/users/{id}` | ✅ | Obtener usuario por Id |

### Productos
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `GET` | `/api/products` | ✅ | Listar todos los productos |
| `GET` | `/api/products/{id}` | ✅ | Obtener producto por Id |
| `GET` | `/api/products/search?term=...` | ✅ | Buscar por nombre |
| `POST` | `/api/products` | ✅ | Crear producto |
| `PUT` | `/api/products/{id}` | ✅ | Actualizar producto |
| `DELETE` | `/api/products/{id}` | ✅ | Eliminar producto |

### Categorías
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `GET` | `/api/categories` | ✅ | Listar categorías |
| `GET` | `/api/categories/{id}` | ✅ | Obtener categoría por Id |
| `POST` | `/api/categories` | ✅ | Crear categoría |
| `DELETE` | `/api/categories/{id}` | ✅ | Eliminar categoría |

### Órdenes
| Método | Ruta | Auth | Descripción |
|---|---|---|---|
| `GET` | `/api/orders/{id}` | ✅ | Obtener orden por Id (con ítems) |
| `POST` | `/api/orders` | ✅ | Crear orden (descuenta stock automáticamente) |

---

## 🗄️ Base de datos

El proyecto usa **SQLite** — no requiere instalar nada adicional.

**Connection string** en `src/Livy.WebApi/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ecommerce.db"
  }
}
```

### Comandos útiles de migraciones

```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion \
  --project Livy/src/Livy.Infrastructure \
  --startup-project Livy/src/Livy.WebApi

# Aplicar migraciones pendientes
dotnet ef database update \
  --project Livy/src/Livy.Infrastructure \
  --startup-project Livy/src/Livy.WebApi

# Revertir todas las migraciones
dotnet ef database update 0 \
  --project Livy/src/Livy.Infrastructure \
  --startup-project Livy/src/Livy.WebApi
```

---

## 🛠️ Tecnologías utilizadas

| Tecnología | Versión | Uso |
|---|---|---|
| ASP.NET Core | 8.0 | Framework web |
| Entity Framework Core | 8.0 | ORM |
| SQLite | — | Base de datos de desarrollo |
| MediatR | 12.x | Patrón CQRS / Mediator |
| JWT Bearer | 8.0 | Autenticación |
| Swashbuckle (Swagger) | 6.6 | Documentación de la API |

---
