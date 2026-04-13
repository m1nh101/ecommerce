using Domain.Primitives;

namespace Domain.Users.Events;

public sealed record UserCreatedDomainEvent(UserId UserId) : IDomainEvent;
