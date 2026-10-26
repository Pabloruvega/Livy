// Application/DTOs/Order/OrderDto.cs
namespace Livy.Application.DTOs.Order;

public class OrderDto
{
    public Guid              Id        { get; set; }
    public Guid              UserId    { get; set; }
    public string            Status    { get; set; } = string.Empty;
    public decimal           Total     { get; set; }
    public DateTime          CreatedAt { get; set; }
    public string?           PaymentTransactionCode { get; set; }
    public string?           PaymentMessage         { get; set; }
    public List<OrderItemDto> Items    { get; set; } = new();
}

public class OrderItemDto
{
    public Guid    Id        { get; set; }
    public Guid    ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public int     Quantity  { get; set; }
    public decimal Subtotal  { get; set; }
}