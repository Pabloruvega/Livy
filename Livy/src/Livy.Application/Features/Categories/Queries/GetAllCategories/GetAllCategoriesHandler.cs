// Application/Features/Categories/Queries/GetAllCategories/GetAllCategoriesHandler.cs
using Livy.Application.Contracts.Persistence;
using Livy.Application.DTOs.Category;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Livy.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesHandler
    : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly ICategoryRepository _repo;
    private readonly ILogger<GetAllCategoriesHandler> _logger;

    public GetAllCategoriesHandler(ICategoryRepository repo, ILogger<GetAllCategoriesHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Obteniendo todas las categorías.");
        var categories = await _repo.GetAllAsync(ct);
        return categories.Select(c => c.ToDto());
    }
}
