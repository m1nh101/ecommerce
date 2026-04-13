using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Users;

public sealed class UserAddress : Entity<UserAddressId>
{
    private UserAddress() { }

    private UserAddress(UserAddressId id, UserId userId, Address address, string? label, bool isDefault)
        : base(id)
    {
        UserId = userId;
        Address = address;
        Label = label;
        IsDefault = isDefault;
        CreatedAt = DateTime.UtcNow;
    }

    public UserId UserId { get; private set; }
    public Address Address { get; private set; } = null!;
    public string? Label { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static UserAddress Create(UserId userId, Address address, string? label, bool isDefault) =>
        new(UserAddressId.New(), userId, address, label, isDefault);

    internal void SetAsDefault() => IsDefault = true;
    internal void ClearDefault() => IsDefault = false;
}
