// Application/UseCases/Products/Commands/CreateProduct/CreateProductCommand.cs
namespace Livy.Application.UseCases.Products.Commands.CreateProduct;


public record CreateProductCommand(
    string  Name,
    string  Description,
    decimal Price,
    int     Stock,
    Guid    CategoryId
);