# Livy — Trabajo Final Backend 2026

> README provisional.

Trabajo Final de Backend 2026 (opción 1): el e-commerce **Livy** y el microservicio **PaymentService**,
dos soluciones .NET 8 independientes que se comunican por HTTP.

## Estructura del repo

```
repo/
├── .gitignore
├── README.md
├── Livy/
│   ├── Livy.sln
│   ├── README.md
│   └── src/ (Livy.Domain, Livy.Application, Livy.Infrastructure, Livy.WebApi)
└── PaymentService/
    ├── PaymentService.sln
    ├── README.md
    └── src/
```

## Cómo levantar los servicios

Desde la raíz del repo, en dos terminales (primero PaymentService, que es el que consume Livy):

```bash
dotnet run --project PaymentService/src/PaymentService.Api   # http://localhost:5090
dotnet run --project Livy/src/Livy.WebApi                     # http://localhost:5204
```

## Documentación de cada servicio

- [Livy (e-commerce)](Livy/README.md)
- [PaymentService](PaymentService/README.md)
