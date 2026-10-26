// WebApi/Controllers/ProductsController.cs
using Livy.Application.DTOs.Product;
using Livy.Application.Features.Products.Commands.ActivateProduct;
using Livy.Application.Features.Products.Commands.CreateProduct;
using Livy.Application.Features.Products.Commands.DeactivateProduct;
using Livy.Application.Features.Products.Commands.DeleteProduct;
using Livy.Application.Features.Products.Commands.UpdateProduct;
using Livy.Application.Features.Products.Queries.GetAllProducts;
using Livy.Application.Features.Products.Queries.GetProductById;
using Livy.Application.Features.Products.Queries.SearchProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Livy.WebApi.Controllers;


/// Flujo Clean Architecture con MediatR:
/// HTTP Request → Controller → _mediator.Send(command/query) → Handler → Repository → BD
/// El Controller NO sabe qué Handler existe — solo envía mensajes.

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    /// Obtiene todos los productos
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllProductsQuery(), ct));

    /// Busca productos cuyo nombre contenga el término dado
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string? term, CancellationToken ct)
        => Ok(await _mediator.Send(new SearchProductsQuery(term ?? string.Empty), ct));

    /// Obtiene un producto por Id
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetProductByIdQuery(id), ct));

    /// Crea un nuevo producto
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var command = new CreateProductCommand(
            dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
        var created = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// Actualiza un producto existente
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            id, dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
        return Ok(await _mediator.Send(command, ct));
    }

    /// Elimina un producto
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }

    /// Activa un producto (IsActive = true)
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new ActivateProductCommand(id), ct));

    /// Desactiva un producto (IsActive = false)
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new DeactivateProductCommand(id), ct));
}