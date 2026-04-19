using Domain.Primitives;
using Mediator;

namespace Application.Products.Images.Update;

public sealed record UpdateImageCommand(
    Guid ProductId,
    Guid ImageId,
    bool IsPrimary) : ICommand<Result>;
