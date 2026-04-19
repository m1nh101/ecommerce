using Domain.Primitives;
using Domain.Products;

namespace Application.Categories;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct);
    Task<Result<Guid>> AddAsync(AddCategoryRequest request, CancellationToken ct);
    Task<Result> UpdateAsync(CategoryId id, UpdateCategoryRequest request, CancellationToken ct);
    Task<Result> DeactivateAsync(CategoryId id, CancellationToken ct);
}
