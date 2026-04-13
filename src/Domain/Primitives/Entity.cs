namespace Domain.Primitives;

public abstract class Entity<TId>
    where TId : notnull
{
    protected Entity(TId id) => Id = id;

    protected Entity() { }

    public TId Id { get; private init; } = default!;

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType()) return false;
        var entity = (Entity<TId>)obj;
        return Id.Equals(entity.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left is not null && right is not null && left.Equals(right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
        !(left == right);
}
