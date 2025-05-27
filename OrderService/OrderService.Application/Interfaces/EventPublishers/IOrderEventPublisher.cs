namespace OrderService.Application.Interfaces.EventPublishers;

public interface IOrderEventPublisher
{
    Task PublishOrderCreatedEvent(Domain.Entities.Order order);
}
