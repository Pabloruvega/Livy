// Application/DTOs/Order/CreateOrderDto.cs
namespace Livy.Application.DTOs.Order;

/// <summary>Request: datos para crear una nueva orden. Validado por CreateOrderCommandValidator (FluentValidation).
/// El UserId no viaja en el body: sale del token del usuario autenticado.</summary>
public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}