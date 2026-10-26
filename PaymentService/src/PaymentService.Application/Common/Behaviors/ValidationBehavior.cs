using FluentValidation;
using MediatR;

namespace PaymentService.Application.Common.Behaviors;

// Pipeline behavior de MediatR: antes de que un Command/Query llegue a su Handler,
// corre todos los IValidator<TRequest> registrados. Si alguno falla, lanza
// FluentValidation.ValidationException y el Handler nunca se ejecuta.
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next();
    }
}
