using System.Security.Claims;
using Application.Abstractions;

namespace Api.Services;

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId =>
        Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
