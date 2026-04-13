namespace Domain.Users;

public readonly record struct UserAddressId(Guid Value)
{
    public static UserAddressId New() => new(Guid.NewGuid());
}
