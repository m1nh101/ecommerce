using Domain.Primitives;
using Domain.Products;

namespace Application.Sizes;

public interface ISizeService
{
    Task<IReadOnlyList<SizeResponse>> GetAllAsync(string? sizeType, CancellationToken ct);
    Task<Result<Guid>> AddAsync(AddSizeRequest request, CancellationToken ct);
    Task<Result> UpdateAsync(SizeId id, UpdateSizeRequest request, CancellationToken ct);
    Task<Result> DeleteAsync(SizeId id, CancellationToken ct);
}
