// Application/Features/Orders/Commands/CreateOrder/CreateOrderCommandValidator.cs
using FluentValidation;

namespace Livy.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("La orden debe tener al menos un ítem.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemCommandValidator());
    }
}

public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemCommand>
{
    public CreateOrderItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser al menos 1.");
    }
}
