// WebApi/Program.cs
using Livy.Application;
using Livy.Infrastructure;
using Livy.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios ────────────────────────────────────────────────────────────────

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Livy E-Commerce API",
        Version     = "v1",
        Description = "API REST para el sistema de e-commerce Livy. Tecnicatura UCCuyo 2026."
    });
    // Incluir comentarios XML de los controllers si existieran
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

// Capas de la arquitectura
builder.Services.AddInfrastructure(builder.Configuration); // DbContext + repositorios
builder.Services.AddApplicationServices();                 // Services de Application

// Logging estructurado — usa el de ASP.NET Core por defecto (console + debug)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ─── Pipeline HTTP ────────────────────────────────────────────────────────────

var app = builder.Build();

// Middleware de logging de requests (va primero para loguear todo)
app.UseMiddleware<RequestLoggingMiddleware>();

// Middleware global de manejo de errores (va segundo para capturar todo)
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Livy API v1");
        c.RoutePrefix = "swagger"; // acceso en /swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();