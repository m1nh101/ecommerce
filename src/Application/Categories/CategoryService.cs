using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Categories;

public sealed class CategoryService(
    IApplicationDbContext db,
    IValidator<AddCategoryRequest> addValidator,
    IValidator<UpdateCategoryRequest> updateValidator) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct)
    {
        return await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryResponse(c.Id.Value, c.Name, c.Slug, c.Description, c.DisplayOrder, c.IsActive))
            .ToListAsync(ct);
    }

    public async Task<Result<Guid>> AddAsync(AddCategoryRequest request, CancellationToken ct)
    {
        var validation = await addValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var exists = await db.Categories.AnyAsync(c => c.Slug == request.Slug, ct);
        if (exists)
            return CategoryErrors.DuplicateSlug;

        var result = Category.Create(request.Name, request.Slug, request.Description, request.DisplayOrder);
        if (result.IsFailure)
            return result.Error;

        db.Categories.Add(result.Value);
        await db.SaveChangesAsync(ct);

        return result.Value.Id.Value;
    }

    public async Task<Result> UpdateAsync(CategoryId id, UpdateCategoryRequest request, CancellationToken ct)
    {
        var validation = await updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null)
            return CategoryErrors.NotFound;

        var slugConflict = await db.Categories
            .AnyAsync(c => c.Slug == request.Slug && c.Id != id, ct);

        if (slugConflict)
            return CategoryErrors.DuplicateSlug;

        category.UpdateDetails(request.Name, request.Slug, request.Description, request.DisplayOrder);

        if (request.IsActive)
            category.Activate();
        else
            category.Deactivate();

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> DeactivateAsync(CategoryId id, CancellationToken ct)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category is null)
            return CategoryErrors.NotFound;

        category.Deactivate();
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
