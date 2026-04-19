using Domain.Primitives;
using Domain.Products;

namespace Application.Colors;

public interface IColorService
{
    Task<IReadOnlyList<ColorResponse>> GetAllAsync(CancellationToken ct);
    Task<Result<Guid>> AddAsync(AddColorRequest request, CancellationToken ct);
    Task<Result> UpdateAsync(ColorId id, UpdateColorRequest request, CancellationToken ct);
    Task<Result> DeleteAsync(ColorId id, CancellationToken ct);
}
