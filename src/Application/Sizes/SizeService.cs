using Application.Abstractions;
using Domain.Enums;
using Domain.Primitives;
using Domain.Products;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Sizes;

public sealed class SizeService(
    IApplicationDbContext db,
    IValidator<AddSizeRequest> addValidator,
    IValidator<UpdateSizeRequest> updateValidator) : ISizeService
{
    public async Task<IReadOnlyList<SizeResponse>> GetAllAsync(string? sizeType, CancellationToken ct)
    {
        var query = db.Sizes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(sizeType) &&
            Enum.TryParse<SizeType>(sizeType, ignoreCase: true, out var parsedType))
        {
            query = query.Where(s => s.SizeType == parsedType);
        }

        return await query
            .OrderBy(s => s.SizeType)
            .ThenBy(s => s.SortOrder)
            .Select(s => new SizeResponse(s.Id.Value, s.Name, s.SizeType.ToString(), s.SortOrder))
            .ToListAsync(ct);
    }

    public async Task<Result<Guid>> AddAsync(AddSizeRequest request, CancellationToken ct)
    {
        var validation = await addValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        if (!Enum.TryParse<SizeType>(request.SizeType, ignoreCase: true, out var sizeType))
            return SizeErrors.InvalidName;

        var exists = await db.Sizes
            .AnyAsync(s => s.Name == request.Name && s.SizeType == sizeType, ct);

        if (exists)
            return SizeErrors.DuplicateNameAndType;

        var result = Size.Create(request.Name, sizeType, request.SortOrder);
        if (result.IsFailure)
            return result.Error;

        db.Sizes.Add(result.Value);
        await db.SaveChangesAsync(ct);

        return result.Value.Id.Value;
    }

    public async Task<Result> UpdateAsync(SizeId id, UpdateSizeRequest request, CancellationToken ct)
    {
        var validation = await updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var size = await db.Sizes.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (size is null)
            return SizeErrors.NotFound;

        size.UpdateSortOrder(request.SortOrder);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(SizeId id, CancellationToken ct)
    {
        var size = await db.Sizes.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (size is null)
            return SizeErrors.NotFound;

        db.Sizes.Remove(size);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
