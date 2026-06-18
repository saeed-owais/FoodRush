namespace FoodRush.Domain.Common;

public record DomainEvent(Guid Id) : IDomainEvent;