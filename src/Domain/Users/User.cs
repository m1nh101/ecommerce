using Domain.Enums;
using Domain.Primitives;
using Domain.ValueObjects;
using Domain.Users.Events;

namespace Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<UserAddress> _addresses = [];

    private User() { }

    private User(UserId id, string email, string firstName, string lastName, UserRole role)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<UserAddress> Addresses => _addresses.AsReadOnly();

    public static User Create(string email, string firstName, string lastName, UserRole role = UserRole.Customer)
    {
        var user = new User(UserId.New(), email, firstName, lastName, role);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        return user;
    }

    public Result AddAddress(Address address, string? label, bool isDefault)
    {
        if (isDefault)
            foreach (var existing in _addresses)
                existing.ClearDefault();

        _addresses.Add(UserAddress.Create(Id, address, label, isDefault));
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveAddress(UserAddressId addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null)
            return Result.Failure(UserErrors.AddressNotFound);

        if (address.IsDefault && _addresses.Count > 1)
            return Result.Failure(UserErrors.CannotRemoveDefaultAddress);

        _addresses.Remove(address);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result SetDefaultAddress(UserAddressId addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null)
            return Result.Failure(UserErrors.AddressNotFound);

        foreach (var a in _addresses)
            a.ClearDefault();

        address.SetAsDefault();
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure(UserErrors.AlreadyInactive);

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new UserDeactivatedDomainEvent(Id));
        return Result.Success();
    }
}
