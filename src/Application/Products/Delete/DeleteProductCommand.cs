using Domain.Primitives;
using Mediator;

namespace Application.Products.Delete;

public sealed record DeleteProductCommand(Guid ProductId) : ICommand<Result>;
