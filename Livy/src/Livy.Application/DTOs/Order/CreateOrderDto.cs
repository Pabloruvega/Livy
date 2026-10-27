// Application/DTOs/Order/CreateOrderDto.cs
namespace Livy.Application.DTOs.Order;

/// <summary>Request: datos para crear una nueva orden. Validado por CreateOrderCommandValidator (FluentValidation).</summary>
public class CreateOrderDto
{
    public Guid UserId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}