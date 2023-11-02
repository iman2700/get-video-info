namespace Domain.Common;

// The BaseAuditableEntity class is an extension of BaseEntity, introducing auditing properties.
// It serves as a foundational component for entities that require auditing of creation and modification information.
public abstract class BaseAuditableEntity : BaseEntity
{
    // Represents the date and time when the entity was created.
    public DateTimeOffset Created { get; set; }

    // Represents the user or source responsible for creating the entity.
    public string? CreatedBy { get; set; }

    // Represents the date and time when the entity was last modified.
    public DateTimeOffset LastModified { get; set; }

    // Represents the user or source responsible for the last modification of the entity.
    public string? LastModifiedBy { get; set; }
}
