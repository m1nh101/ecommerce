using Application.Common;
using Domain.Primitives;
using Mediator;

namespace Application.Products.Get;

public sealed record GetProductsQuery(
    string? Name,
    string? Brand,
    Guid? CategoryId,
    string? Gender,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? IsActive,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<Result<PagedResponse<ProductListItemResponse>>>;
