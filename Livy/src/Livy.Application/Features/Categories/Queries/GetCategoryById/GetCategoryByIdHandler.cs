// Application/Features/Categories/Queries/GetCategoryById/GetCategoryByIdHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Category;
using Livy.Domain.Exceptions;
using MediatR;

namespace Livy.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _repo;

    public GetCategoryByIdHandler(ICategoryRepository repo) => _repo = repo;

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await _repo.GetByIdAsync(request.CategoryId, ct)
            ?? throw new CategoryNotFoundException(request.CategoryId);

        return category.ToDto();
    }
}
