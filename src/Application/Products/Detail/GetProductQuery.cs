using Domain.Primitives;
using Mediator;

namespace Application.Products.Detail;

public sealed record GetProductQuery(Guid ProductId) : IQuery<Result<ProductDetailResponse>>;
