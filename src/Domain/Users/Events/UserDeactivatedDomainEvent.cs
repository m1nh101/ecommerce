using Domain.Primitives;

namespace Domain.Users.Events;

public sealed record UserDeactivatedDomainEvent(UserId UserId) : IDomainEvent;
