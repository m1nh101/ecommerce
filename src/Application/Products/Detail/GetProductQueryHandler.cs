using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Detail;

public sealed class GetProductQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetProductQuery, Result<ProductDetailResponse>>
{
    public async ValueTask<Result<ProductDetailResponse>> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        var productId = new ProductId(query.ProductId);
        var product = await dbContext.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        var variants = product.Variants
            .Select(v => new ProductVariantResponse(
                v.Id.Value,
                v.ColorId.Value,
                v.SizeId.Value,
                v.Sku,
                v.PriceOverride?.Amount,
                v.PriceOverride?.Currency,
                v.StockQuantity,
                v.StockQuantity == 0 ? "OutOfStock" : v.StockQuantity <= 5 ? "LowStock" : "InStock",
                v.IsActive))
            .ToList();

        var images = product.Images
            .Select(i => new ProductImageResponse(
                i.Id.Value,
                i.VariantId?.Value,
                i.Url,
                i.IsPrimary,
                i.SortOrder))
            .ToList();

        return new ProductDetailResponse(
            product.Id.Value,
            product.Name,
            product.Description,
            product.Brand,
            product.CategoryId.Value,
            product.Gender.ToString(),
            product.BasePrice.Amount,
            product.BasePrice.Currency,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt,
            variants,
            images);
    }
}
