using PaymentService.Api.Middleware;
using PaymentService.Application;
using PaymentService.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ─── Servicios ────────────────────────────────────────────────────────────────

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Livy PaymentService API",
        Version = "v1",
        Description = "Microservicio de pagos, consumido por el e-commerce Livy vía HttpClient."
    });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Clave compartida con el e-commerce. Ver appsettings 'Security:ApiKey'."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Capas de la arquitectura
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ─── Pipeline HTTP ────────────────────────────────────────────────────────────

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentService API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
