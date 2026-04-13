using Domain.Primitives;

namespace Domain.Users;

public static class UserErrors
{
    public static readonly Error AddressNotFound = new("User.AddressNotFound", "The specified address was not found.");
    public static readonly Error CannotRemoveDefaultAddress = new("User.CannotRemoveDefaultAddress", "Cannot remove the default address while other addresses exist. Set a new default first.");
    public static readonly Error AlreadyInactive = new("User.AlreadyInactive", "The user is already inactive.");
    public static readonly Error NotFound = new("User.NotFound", "The user was not found.");
    public static readonly Error EmailAlreadyInUse = new("User.EmailAlreadyInUse", "A user with this email already exists.");
}
