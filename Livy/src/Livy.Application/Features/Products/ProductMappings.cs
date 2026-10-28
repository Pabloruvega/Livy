// Application/Features/Products/ProductMappings.cs
using Livy.Application.DTOs.Product;
using Livy.Domain.Entities;

namespace Livy.Application.Features.Products;

// Único mapeo Product → ProductDto, compartido por todos los handlers de Products.
public static class ProductMappings
{
    public static ProductDto ToDto(this Product product) => new()
    {
        Id          = product.Id,
        Name        = product.Name,
        Description = product.Description,
        Price       = product.Price,
        Stock       = product.Stock,
        CategoryId  = product.CategoryId,
        CreatedAt   = product.CreatedAt,
        IsActive    = product.IsActive
    };
}
