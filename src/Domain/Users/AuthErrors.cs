using Domain.Primitives;

namespace Domain.Users;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "The email or password is incorrect.");

    public static Error AccountLockedOut(DateTimeOffset lockedUntil) =>
        new("Auth.AccountLockedOut", $"Too many failed attempts. Account is locked until {lockedUntil:u}.");
}
