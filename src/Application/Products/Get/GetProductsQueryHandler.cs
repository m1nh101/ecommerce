using Application.Abstractions;
using Application.Common;
using Domain.Primitives;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Get;

public sealed class GetProductsQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetProductsQuery, Result<PagedResponse<ProductListItemResponse>>>
{
    public async ValueTask<Result<PagedResponse<ProductListItemResponse>>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var productsQuery = dbContext.Products
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Name))
            productsQuery = productsQuery.Where(p => p.Name.Contains(query.Name));

        if (query.MinPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.Price.Amount >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.Price.Amount <= query.MaxPrice.Value);

        if (query.InStock.HasValue)
            productsQuery = query.InStock.Value
                ? productsQuery.Where(p => p.StockQuantity > 0)
                : productsQuery.Where(p => p.StockQuantity == 0);

        var totalCount = await productsQuery.CountAsync(cancellationToken);

        var items = await productsQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductListItemResponse(
                p.Id.Value,
                p.Name,
                p.Description,
                p.Price.Amount,
                p.Price.Currency,
                p.StockQuantity,
                p.Category,
                p.IsActive,
                p.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProductListItemResponse>(items, query.PageNumber, query.PageSize, totalCount);
    }
}
