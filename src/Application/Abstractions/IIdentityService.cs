using Domain.Primitives;

namespace Application.Abstractions;

public interface IIdentityService
{
    Task<Result<(string UserId, IReadOnlyList<string> Roles)>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);
}
