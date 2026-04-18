using Application.Abstractions;
using Application.Common;
using Domain.Enums;
using Domain.Primitives;
using Domain.Products;
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
        var productsQuery = dbContext.Products.AsNoTracking();

        if (query.IsActive.HasValue)
            productsQuery = productsQuery.Where(p => p.IsActive == query.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(query.Name))
            productsQuery = productsQuery.Where(p => p.Name.Contains(query.Name));

        if (!string.IsNullOrWhiteSpace(query.Brand))
            productsQuery = productsQuery.Where(p => p.Brand.Contains(query.Brand));

        if (query.CategoryId.HasValue)
        {
            var categoryId = new CategoryId(query.CategoryId.Value);
            productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(query.Gender) && Enum.TryParse<Gender>(query.Gender, ignoreCase: true, out var gender))
            productsQuery = productsQuery.Where(p => p.Gender == gender);

        if (query.MinPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.BasePrice.Amount >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.BasePrice.Amount <= query.MaxPrice.Value);

        var totalCount = await productsQuery.CountAsync(cancellationToken);

        var items = await productsQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductListItemResponse(
                p.Id.Value,
                p.Name,
                p.Description,
                p.Brand,
                p.CategoryId.Value,
                p.Gender.ToString(),
                p.BasePrice.Amount,
                p.BasePrice.Currency,
                p.IsActive,
                p.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResponse<ProductListItemResponse>(items, query.PageNumber, query.PageSize, totalCount);
    }
}
