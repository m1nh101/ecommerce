using Domain.Primitives;
using Mediator;

namespace Application.Products.Images.Add;

public sealed record AddImageCommand(
    Guid ProductId,
    string Url,
    Guid? VariantId,
    bool IsPrimary,
    int SortOrder) : ICommand<Result<Guid>>;
