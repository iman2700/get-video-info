using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common;

// The BaseEntity class is a foundational component for entities in the domain.
// It provides a basic structure for entities with a numeric identifier, which can be extended for other key types.
public abstract class BaseEntity
{
    // This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
    // Using non-generic integer types for simplicity
    public int Id { get; set; }

    private readonly List<BaseEvent> _domainEvents = new();

    // Gets a read-only collection of domain events associated with this entity.
    [NotMapped] // Indicates that this property should not be mapped to the database.
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Adds a domain event to the list of domain events associated with this entity.
    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    // Removes a domain event from the list of domain events associated with this entity.
    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    // Clears all domain events associated with this entity.
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
