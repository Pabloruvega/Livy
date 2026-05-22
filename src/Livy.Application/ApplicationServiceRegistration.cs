// Application/ApplicationServiceRegistration.cs
using Livy.Application.Common;
using Livy.Application.DTOs.Order;
using Livy.Application.DTOs.Product;
using Livy.Application.Services;
using Livy.Application.UseCases.Orders.Commands.CreateOrder;
using Livy.Application.UseCases.Orders.Queries.GetOrderById;
using Livy.Application.UseCases.Products.Commands.CreateProduct;
using Livy.Application.UseCases.Products.Commands.DeleteProduct;
using Livy.Application.UseCases.Products.Commands.UpdateProduct;
using Livy.Application.UseCases.Products.Queries.GetAllProducts;
using Livy.Application.UseCases.Products.Queries.GetProductById;
using Microsoft.Extensions.DependencyInjection;

namespace Livy.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // ── Casos de uso: Products — Queries (solo lectura) ─────────────────
        services.AddScoped<
            IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>,
            GetAllProductsHandler>();

        services.AddScoped<
            IQueryHandler<GetProductByIdQuery, ProductDto>,
            GetProductByIdHandler>();

        // ── Casos de uso: Products — Commands (modifican estado) ────────────
        services.AddScoped<
            ICommandHandler<CreateProductCommand, ProductDto>,
            CreateProductHandler>();

        services.AddScoped<
            ICommandHandler<UpdateProductCommand, ProductDto>,
            UpdateProductHandler>();

        services.AddScoped<
            ICommandHandler<DeleteProductCommand, bool>,
            DeleteProductHandler>();

        // ── Casos de uso: Orders ─────────────────────────────────────────────
        services.AddScoped<
            ICommandHandler<CreateOrderCommand, OrderDto>,
            CreateOrderHandler>();

        services.AddScoped<
            IQueryHandler<GetOrderByIdQuery, OrderDto>,
            GetOrderByIdHandler>();

        // ── Services: User y Category ────────────────────────────────────────
        // Login/Register y gestión de categorías mantienen el patrón Service
        services.AddScoped<IUserService,     UserService>();
        services.AddScoped<ICategoryService, CategoryService>();

        return services;
    }
}