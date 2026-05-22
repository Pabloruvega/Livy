// Application/Services/ProductService.cs
using Livy.Application.DTOs.Product;
using Livy.Application.Interfaces;
using Livy.Domain.Entities;
using Livy.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repo, ILogger<ProductService> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Obteniendo todos los productos.");
        var products = await _repo.GetAllAsync(ct);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Buscando producto con Id {Id}.", id);
        var product = await _repo.GetByIdAsync(id, ct);
        if (product is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado.", id);
            return null;
        }
        return MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> SearchByNameAsync(string term, CancellationToken ct = default)
    {
        _logger.LogInformation("Buscando productos con término '{Term}'.", term);
        var products = await _repo.SearchByNameAsync(term, ct);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creando producto '{Name}'.", dto.Name);
        var product = new Product(dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
        await _repo.AddAsync(product, ct);
        _logger.LogInformation("Producto creado con Id {Id}.", product.Id);
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Actualizando producto con Id {Id}.", id);
        var product = await _repo.GetByIdAsync(id, ct)
            ?? throw new ProductNotFoundException(id);

        product.UpdatePrice(dto.Price);
        await _repo.UpdateAsync(product, ct);
        _logger.LogInformation("Producto {Id} actualizado correctamente.", id);
        return MapToDto(product);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Eliminando producto con Id {Id}.", id);

        // IRepository<T>.DeleteAsync recibe la entidad, no el Guid
        var product = await _repo.GetByIdAsync(id, ct)
            ?? throw new ProductNotFoundException(id);

        await _repo.DeleteAsync(product, ct); // ← entidad, no id
        _logger.LogInformation("Producto {Id} eliminado.", id);
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id          = p.Id,
        Name        = p.Name,
        Description = p.Description,
        Price       = p.Price,
        Stock       = p.Stock,
        CategoryId  = p.CategoryId,
        CreatedAt   = p.CreatedAt
    };
}