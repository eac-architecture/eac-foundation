using System.Runtime.CompilerServices;

namespace EAC.Foundation.Domain;

/// <summary>
/// Provides optional identity-based equality for a domain entity.
/// </summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>
    /// Initializes a transient entity for use by materializers.
    /// </summary>
    protected Entity()
    {
        Id = default!;
    }

    /// <summary>
    /// Initializes an entity with its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    protected Entity(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    /// <inheritdoc />
    public TId Id { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the identifier has its default value.
    /// </summary>
    public bool IsTransient => EqualityComparer<TId>.Default.Equals(Id, default!);

    /// <inheritdoc />
    public bool Equals(Entity<TId>? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null || GetType() != other.GetType() || IsTransient || other.IsTransient)
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);

    /// <inheritdoc />
    public override int GetHashCode() => IsTransient
        ? RuntimeHelpers.GetHashCode(this)
        : HashCode.Combine(GetType(), Id);

    /// <summary>Determines whether two entities are equal.</summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>Determines whether two entities are different.</summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}
