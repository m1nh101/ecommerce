using Domain.Primitives;
using Mediator;

namespace Application.Products.Images.Remove;

public sealed record RemoveImageCommand(Guid ProductId, Guid ImageId) : ICommand<Result>;
