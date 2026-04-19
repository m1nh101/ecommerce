using Domain.Primitives;
using Mediator;

namespace Application.Products.Variants.Remove;

public sealed record RemoveVariantCommand(Guid ProductId, Guid VariantId) : ICommand<Result>;
