// Application/DTOs/Order/CreateOrderDto.cs
using System.ComponentModel.DataAnnotations;

namespace Livy.Application.DTOs.Order;

/// <summary>Request: datos para crear una nueva orden</summary>
public class CreateOrderDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required, MinLength(1, ErrorMessage = "La orden debe tener al menos un ítem.")]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Quantity { get; set; }
}