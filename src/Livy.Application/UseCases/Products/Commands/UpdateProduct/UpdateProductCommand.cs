// Application/UseCases/Products/Commands/UpdateProduct/UpdateProductCommand.cs
namespace Livy.Application.UseCases.Products.Commands.UpdateProduct;


public record UpdateProductCommand(
    Guid    ProductId,
    string  Name,
    string  Description,
    decimal Price,
    int     Stock,
    Guid    CategoryId
);