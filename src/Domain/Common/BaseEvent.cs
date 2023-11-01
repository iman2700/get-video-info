using MediatR;

namespace Domain.Common;

// The BaseEvent class is a foundational component for handling events in the domain.
// It uses the MediatR library for event-driven communication and implements the INotification interface.
// This abstract class serves as a base for creating specific event types, promoting structured event handling within the domain layer.
public abstract class BaseEvent : INotification
{
}
