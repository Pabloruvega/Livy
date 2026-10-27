// Application/Features/Orders/Commands/PayOrder/PayOrderCommandValidator.cs
using FluentValidation;

namespace Livy.Application.Features.Orders.Commands.PayOrder;

public class PayOrderCommandValidator : AbstractValidator<PayOrderCommand>
{
    public PayOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.RequestingUserId)
            .NotEmpty();
    }
}
