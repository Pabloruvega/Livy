// WebApi/Controllers/ProductsController.cs
using Livy.Application.Common;
using Livy.Application.DTOs.Product;
using Livy.Application.UseCases.Products.Commands.CreateProduct;
using Livy.Application.UseCases.Products.Commands.DeleteProduct;
using Livy.Application.UseCases.Products.Commands.UpdateProduct;
using Livy.Application.UseCases.Products.Queries.GetAllProducts;
using Livy.Application.UseCases.Products.Queries.GetProductById;
using Microsoft.AspNetCore.Mvc;

namespace Livy.WebApi.Controllers;

/// <summary>
/// Flujo correcto Clean Architecture:
/// HTTP Request → Controller → Handler (UseCase) → Repository → BD
///                           ↑ IUnitOfWork persiste al final
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>  _getAllHandler;
    private readonly IQueryHandler<GetProductByIdQuery, ProductDto>               _getByIdHandler;
    private readonly ICommandHandler<CreateProductCommand, ProductDto>            _createHandler;
    private readonly ICommandHandler<UpdateProductCommand, ProductDto>            _updateHandler;
    private readonly ICommandHandler<DeleteProductCommand, bool>                  _deleteHandler;

    public ProductsController(
        IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto>>  getAllHandler,
        IQueryHandler<GetProductByIdQuery, ProductDto>               getByIdHandler,
        ICommandHandler<CreateProductCommand, ProductDto>            createHandler,
        ICommandHandler<UpdateProductCommand, ProductDto>            updateHandler,
        ICommandHandler<DeleteProductCommand, bool>                  deleteHandler)
    {
        _getAllHandler   = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler  = createHandler;
        _updateHandler  = updateHandler;
        _deleteHandler  = deleteHandler;
    }

    /// <summary>Obtiene todos los productos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _getAllHandler.HandleAsync(new GetAllProductsQuery(), ct));

    /// <summary>Obtiene un producto por Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _getByIdHandler.HandleAsync(new GetProductByIdQuery(id), ct));

    /// <summary>Crea un nuevo producto.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var command = new CreateProductCommand(
            dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
        var created = await _createHandler.HandleAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Actualiza un producto existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            id, dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId);
        var updated = await _updateHandler.HandleAsync(command, ct);
        return Ok(updated);
    }

    /// <summary>Elimina un producto.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _deleteHandler.HandleAsync(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}