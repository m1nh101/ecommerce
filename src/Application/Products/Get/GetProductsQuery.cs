using Application.Common;
using Domain.Primitives;
using Mediator;

namespace Application.Products.Get;

public sealed record GetProductsQuery(
    string? Name,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStock,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<Result<PagedResponse<ProductListItemResponse>>>;
