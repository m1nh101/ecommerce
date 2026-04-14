using Application.Abstractions;
using Domain.Primitives;
using Domain.Users;
using Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

internal sealed class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<Result<(string UserId, IReadOnlyList<string> Roles)>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return AuthErrors.InvalidCredentials;

        if (await userManager.IsLockedOutAsync(user))
        {
            var lockedUntil = user.LockoutEnd ?? DateTimeOffset.UtcNow;
            return AuthErrors.AccountLockedOut(lockedUntil);
        }

        var passwordValid = await userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            await userManager.AccessFailedAsync(user);
            return AuthErrors.InvalidCredentials;
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        return (user.Id.ToString(), roles.ToList().AsReadOnly());
    }

    public async Task<Result<Guid>> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
            return UserErrors.EmailAlreadyInUse;

        var identityUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(identityUser, password);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return new Error(error.Code, error.Description);
        }

        return identityUser.Id;
    }
}
