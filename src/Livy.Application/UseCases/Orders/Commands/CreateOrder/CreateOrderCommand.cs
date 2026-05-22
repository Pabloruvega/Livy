// Application/UseCases/Orders/Commands/CreateOrder/CreateOrderCommand.cs
namespace Livy.Application.UseCases.Orders.Commands.CreateOrder;


public record CreateOrderCommand(
    Guid UserId,
    List<CreateOrderItemCommand> Items
);

public record CreateOrderItemCommand(Guid ProductId, int Quantity);