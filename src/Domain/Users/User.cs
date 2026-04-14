using Domain.Primitives;
using Domain.ValueObjects;
using Domain.Users.Events;

namespace Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<UserAddress> _addresses = [];

    private User() { }

    private User(UserId id, Guid identityId, string email, string firstName, string lastName)
        : base(id)
    {
        IdentityId = identityId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public Guid IdentityId { get; private set; }
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<UserAddress> Addresses => _addresses.AsReadOnly();

    public static User Create(Guid identityId, string email, string firstName, string lastName)
    {
        var user = new User(UserId.New(), identityId, email, firstName, lastName);
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
