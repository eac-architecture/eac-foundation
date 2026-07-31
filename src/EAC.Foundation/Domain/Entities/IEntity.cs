namespace EAC.Foundation.Domain;

/// <summary>
/// Exposes the stable identity of a domain entity without imposing persistence concerns.
/// </summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public interface IEntity<out TId>
    where TId : notnull
{
    /// <summary>Gets the entity identifier.</summary>
    TId Id { get; }
}
