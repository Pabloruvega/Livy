# 🛒 Livy — E-Commerce API

API REST desarrollada con **ASP.NET Core 8** y **Clean Architecture** para la materia Backend (Optativa II) — Tecnicatura Universitaria en Desarrollo de Software · UCCuyo 2026.

---

## 🏗️ Arquitectura

```
Livy/
├── Livy.Domain/          # Entidades, Value Objects, Excepciones, Interfaces
├── Livy.Application/     # DTOs, Services, Interfaces de repositorios
├── Livy.Infrastructure/  # EF Core, Repositorios, Migraciones, DbContext
└── Livy.WebApi/          # Controllers, Middleware, Program.cs
```

---

## ✅ Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [DB Browser for SQLite](https://sqlitebrowser.org/) *(opcional, para inspeccionar la BD)*
- [Git](https://git-scm.com/)

---

## 🚀 Cómo levantar el proyecto

### 1. Clonar el repositorio

```bash
git clone https://github.com/Pabloruvega/livy.git
cd livy
```

### 2. Restaurar paquetes NuGet

```bash
dotnet restore
```

### 3. Instalar la herramienta de migraciones *(solo la primera vez)*

```bash
dotnet tool install --global dotnet-ef
```

### 4. Aplicar las migraciones y crear la base de datos

```bash
dotnet ef database update \
  --project src/Livy.Infrastructure \
  --startup-project src/Livy.WebApi
```

> Esto crea automáticamente el archivo `ecommerce.db` en la carpeta `src/Livy.WebApi/`.

### 5. Ejecutar el proyecto

```bash
dotnet run --project src/Livy.WebApi
```

### 6. Abrir Swagger

```
http://localhost:5204/swagger
```

---

## 🔐 Cómo probar el Login

### Paso 1 — Registrar un usuario

**`POST /api/users/register`**

```json
{
  "email": "pablo@gmail.com",
  "name": "Pablo",
  "password": "123456"
}
```

Respuesta esperada `201 Created`:

```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "email": "pablo@gmail.com",
  "name": "Pablo",
  "role": "User",
  "createdAt": "2026-05-21T..."
}
```

---

### Paso 2 — Hacer login

**`POST /api/users/login`**

```json
{
  "email": "pablo@gmail.com",
  "password": "123456"
}
```

Respuesta esperada `200 OK`:

```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "email": "pablo@gmail.com",
  "name": "Pablo",
  "role": "User",
  "createdAt": "2026-05-21T..."
}
```

---

### Casos de error

| Escenario | Status | Mensaje |
|---|---|---|
| Contraseña incorrecta | `401 Unauthorized` | `"Credenciales inválidas."` |
| Email ya registrado | `400 Bad Request` | `"El email '...' ya está registrado."` |
| Campos vacíos | `400 Bad Request` | Mensajes de validación por campo |

---

## 📦 Endpoints disponibles

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/users/register` | Registrar nuevo usuario |
| `POST` | `/api/users/login` | Autenticarse |
| `GET` | `/api/users` | Listar usuarios |
| `GET` | `/api/users/{id}` | Obtener usuario por Id |
| `GET` | `/api/products` | Listar productos |
| `GET` | `/api/products/{id}` | Obtener producto por Id |
| `GET` | `/api/products/search?term=...` | Buscar productos por nombre |
| `POST` | `/api/products` | Crear producto |
| `PUT` | `/api/products/{id}` | Actualizar producto |
| `DELETE` | `/api/products/{id}` | Eliminar producto |
| `GET` | `/api/categories` | Listar categorías |
| `GET` | `/api/categories/{id}` | Obtener categoría por Id |
| `POST` | `/api/categories` | Crear categoría |
| `DELETE` | `/api/categories/{id}` | Eliminar categoría |
| `GET` | `/api/orders/{id}` | Obtener orden por Id |
| `GET` | `/api/orders/user/{userId}` | Órdenes de un usuario |
| `POST` | `/api/orders` | Crear orden |

---

## 🗄️ Base de datos

El proyecto usa **SQLite** con Entity Framework Core. La connection string está en `src/Livy.WebApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ecommerce.db"
  }
}
```

Para inspeccionar la BD visualmente, abrí el archivo `ecommerce.db` con [DB Browser for SQLite](https://sqlitebrowser.org/).

### Comandos útiles de migraciones

```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion \
  --project src/Livy.Infrastructure \
  --startup-project src/Livy.WebApi

# Aplicar migraciones pendientes
dotnet ef database update \
  --project src/Livy.Infrastructure \
  --startup-project src/Livy.WebApi

# Revertir todas las migraciones
dotnet ef database update 0 \
  --project src/Livy.Infrastructure \
  --startup-project src/Livy.WebApi
```

---

## 🛠️ Tecnologías utilizadas

- **ASP.NET Core 8** — Framework web
- **Entity Framework Core 8** — ORM
- **SQLite** — Base de datos de desarrollo
- **Swashbuckle / Swagger** — Documentación de la API
- **Clean Architecture** — Separación en capas Domain / Application / Infrastructure / WebApi

---

## 👨‍💻 Autor

Pablo · Tecnicatura Universitaria en Desarrollo de Software · UCCuyo · 2026  
Profesor: Tec. Nicolás Ortiz
