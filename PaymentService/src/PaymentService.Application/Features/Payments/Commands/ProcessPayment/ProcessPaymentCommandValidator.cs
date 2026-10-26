using FluentValidation;

namespace PaymentService.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");
    }
}
