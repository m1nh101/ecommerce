using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Colors;

public sealed class ColorService(
    IApplicationDbContext db,
    IValidator<AddColorRequest> addValidator,
    IValidator<UpdateColorRequest> updateValidator) : IColorService
{
    public async Task<IReadOnlyList<ColorResponse>> GetAllAsync(CancellationToken ct)
    {
        return await db.Colors
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new ColorResponse(c.Id.Value, c.Name, c.HexCode))
            .ToListAsync(ct);
    }

    public async Task<Result<Guid>> AddAsync(AddColorRequest request, CancellationToken ct)
    {
        var validation = await addValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var exists = await db.Colors.AnyAsync(c => c.Name == request.Name, ct);
        if (exists)
            return ColorErrors.DuplicateName;

        var result = Color.Create(request.Name, request.HexCode);
        if (result.IsFailure)
            return result.Error;

        db.Colors.Add(result.Value);
        await db.SaveChangesAsync(ct);

        return result.Value.Id.Value;
    }

    public async Task<Result> UpdateAsync(ColorId id, UpdateColorRequest request, CancellationToken ct)
    {
        var validation = await updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var color = await db.Colors.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (color is null)
            return ColorErrors.NotFound;

        var result = color.UpdateHexCode(request.HexCode);
        if (result.IsFailure)
            return result.Error;

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(ColorId id, CancellationToken ct)
    {
        var color = await db.Colors.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (color is null)
            return ColorErrors.NotFound;

        db.Colors.Remove(color);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
