// Application/Features/Categories/Commands/CreateCategory/CreateCategoryCommandValidator.cs
using FluentValidation;

namespace Livy.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100);
    }
}
