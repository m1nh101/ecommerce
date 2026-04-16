using Domain.Carts;

namespace Application.Abstractions;

public interface ICartStore
{
    Task<Cart?> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveAsync(Cart cart, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
}
