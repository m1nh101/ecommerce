using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Mediator;

namespace Application.Products.Detail;

public sealed class GetProductQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetProductQuery, Result<ProductDetailResponse>>
{
    public async ValueTask<Result<ProductDetailResponse>> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([new ProductId(query.ProductId)], cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        return new ProductDetailResponse(
            product.Id.Value,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity,
            product.Category,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
